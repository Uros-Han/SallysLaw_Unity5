//
//  StoreKitX_ios.mm
//  BeautyCrush
//
//  Created by Sinhyub Kim on 3/14/14.
//
//

#import "UnityAppController.h"

#include "StoreKitX.h"
#import <StoreKit/StoreKit.h>
#import <CommonCrypto/CommonCrypto.h>
#include "VerificationController.h"

#include "UserDefaultManager.h"
#import "Reachability.h"

//#import "../Libraries/FuseUnitySDK.h"

StoreKitX* StoreKitX::_instance = NULL;

@interface StoreKitXDelegateBridge : NSObject<SKProductsRequestDelegate, SKPaymentTransactionObserver>
@end
static StoreKitXDelegateBridge* s_StoreKitXDelegateBridge = nil;
static SKPaymentTransaction* s_CurrentPaymentTransaction = nil;


StoreKitX::StoreKitX():m_Delegate(NULL),m_IsProcessingPurchase(false),m_AccountKey("")
{
}

StoreKitX::~StoreKitX()
{

}

StoreKitX* StoreKitX::sharedStoreKitX()
{
    if( _instance == NULL )
    {
        _instance = new StoreKitX();
        s_StoreKitXDelegateBridge = [[StoreKitXDelegateBridge alloc] init];
    }
    
    return _instance;
}

static const std::string Key_StoreKitX_IsProcessingPurchase = "StoreKitX_IsProcessingPurchase";
static const std::string Key_StoreKitX_ProcssingStoreItemID = "StoreKitX_ProcessingStoreItemID";
static const std::string Key_StoreKitX_ProcssingTransactionID = "StoreKitX_ProcessingTransactionID";

bool StoreKitX::Initialize(StoreKitXDelegate* delegate, std::string accountKey)
{
    m_IsBeginPurchase = false;
    this->SetDelegate(delegate);
    m_AccountKey = accountKey;

    m_StoreItemRequestCount = 0;
    // Check if there remains any purchases to process...
    m_IsProcessingPurchase = UserDefaultManager::GetInstance().GetBoolForKey(Key_StoreKitX_IsProcessingPurchase.c_str(), false);
    if (m_IsProcessingPurchase) {
        return true;
    }

    m_IsProcessingErrorPurchase = false;
    
    [[SKPaymentQueue defaultQueue] addTransactionObserver:s_StoreKitXDelegateBridge];
    
    return false;
}

void StoreKitX::ProcessErrorPurchase()
{
    if( m_IsProcessingPurchase == true )
    {
        std::string currentProcessingStoreItemID = UserDefaultManager::GetInstance().GetStringForKey(Key_StoreKitX_ProcssingStoreItemID.c_str());
        std::string currentProcessingTransactionID = UserDefaultManager::GetInstance().GetStringForKey(Key_StoreKitX_ProcssingTransactionID.c_str());
        
        //        [[SKPaymentQueue defaultQueue] restoreCompletedTransactions];
        
        NSLog(@"Remained Item [%s]", currentProcessingStoreItemID.c_str());
        
        this->SetProcessingPurchase(false, currentProcessingStoreItemID, "0");

        this->OnProcessErrorPurchase(currentProcessingStoreItemID.c_str(), currentProcessingTransactionID.c_str());
        
        m_IsProcessingErrorPurchase = true;
        
        //
        [[SKPaymentQueue defaultQueue] addTransactionObserver:s_StoreKitXDelegateBridge];
    }
}

void StoreKitX::SetProcessingPurchase(bool isProcessingPurchase, std::string storeItemID, std::string transactionID)
{
    if( isProcessingPurchase == false )
        storeItemID = "none";
    
    NSLog(@"SetProcessingPurchase: %s", storeItemID.c_str());

    UserDefaultManager::GetInstance().SetBoolForKey(Key_StoreKitX_IsProcessingPurchase.c_str(), isProcessingPurchase);
    UserDefaultManager::GetInstance().SetStringForKey(Key_StoreKitX_ProcssingStoreItemID.c_str(), storeItemID);
    UserDefaultManager::GetInstance().SetStringForKey(Key_StoreKitX_ProcssingTransactionID.c_str(), transactionID);
    UserDefaultManager::GetInstance().Flush();
    
    m_IsProcessingPurchase = isProcessingPurchase;
}

int StoreKitX::BeginPurchase(StoreItemCategory storeItemCategory, std::string storeItemID)
{
    m_IsBeginPurchase=true;
    if (StoreKitX::sharedStoreKitX()->getStoreItemRequestCount() > 0) {
        // Error popup
        UIAlertView* alertView = [[UIAlertView alloc] initWithTitle:@"In-App Purchases" message:@"Retrieving product information, Please try again." delegate:nil cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
        [alertView release];

        m_Delegate->OnErrorPurchaseBegin();
        NSLog(@"not complete get item information");
        return 1;
    }
    
    /////Internet access check//// Modified by Uros 2016.07.11
    /////만약 인터넷 연결 안되어있으면 에러 메시지박스 띄우고 결제 종료.
    /////#import "Reachability.h" 추가
    /////Reachability 클래스 추가.
    /////SystemConfiguration 프레임워크 추가.
    NetworkStatus netStatus = [[Reachability reachabilityForInternetConnection] currentReachabilityStatus];
    if(netStatus == NotReachable)
    {
        // Error popup
        UIAlertView* alertView = [[UIAlertView alloc] initWithTitle:@"In-App Purchases" message:@"Please connect to the Internet." delegate:nil cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
        [alertView release];
        
        m_Delegate->OnErrorPurchaseBegin();
        NSLog(@"Internet connection error");
        return 1;
    }

    NSLog(@"[StoreKitX] BeginPurchase : %s\n", storeItemID.c_str());
    if( [SKPaymentQueue canMakePayments] == false )
    {
        showCannotMakePurchasePopup();
        NSLog(@"don't use purchase");
        return 3;
    }
    
    
    if( IsProcessingPurchase() == true )
    {
        NSLog(@"StoreKitX is processing another Purchase. Can't make another request, until the first Purchasing Process is finished");
        return 2;
    }

    //
    // INIT
    //
    m_IsProcessingPurchase = true;
    m_IsProcessingErrorPurchase = false;
    
    SKProductsRequest* productsRequest = [[SKProductsRequest alloc] initWithProductIdentifiers:[NSSet setWithObject:[NSString stringWithUTF8String:storeItemID.c_str()]]];
    productsRequest.delegate = s_StoreKitXDelegateBridge;
    [productsRequest start];
    
    NSLog(@"success");
    return 0;
}

bool StoreKitX::GetStoreItemInformation(std::string storeItemID)
{
    if( [SKPaymentQueue canMakePayments] == false )
        return false;

    if( IsProcessingPurchase() == true )
    {
        NSLog(@"StoreKitX is processing another Purchase. Can't make another request, until the first Purchasing Process is finished");
        return false;
    }

    //
    // INIT
    //
    m_IsProcessingPurchase = false;
    m_IsProcessingErrorPurchase = false;
    m_StoreItemRequestCount++;
    
    NSSet *produects = [NSSet setWithObjects:
                        @"com.sally.costume",
                        nil];

    SKProductsRequest* productsRequest = [[SKProductsRequest alloc] initWithProductIdentifiers:produects];
    productsRequest.delegate = s_StoreKitXDelegateBridge;
    [productsRequest start];

    return true;
}

bool StoreKitX::FinishPurchase(StoreItemCategory storeItemCategory, std::string storeItemID)
{
    NSLog(@"[StoreKitX] FinishPurchase : %s\n", storeItemID.c_str());
    if( [SKPaymentQueue canMakePayments] == false )
    {
        showCannotMakePurchasePopup();
        return false;
    }
    
    if( IsProcessingPurchase() == false )
    {
        NSLog(@"StoreKitX has no Purchasing Process. Just Process anyway...");
    }
    
    SKPaymentTransaction* transaction = s_CurrentPaymentTransaction;
    
    if( transaction == nil )
    {
        NSLog(@"Three is no PaymentTransaction on Processing. Quit the Request FinishPurchase");
        return false;
    }
    
    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
    
    std::string transactionID = UserDefaultManager::GetInstance().GetStringForKey(Key_StoreKitX_ProcssingTransactionID.c_str());
    this->OnFinishPurchaseResponse(storeItemID.c_str(), storeItemID.length(), transactionID.c_str());
    return true;
}

void StoreKitX::showCannotMakePurchasePopup()
{
    NSString* invalidIdentifier = @"StoreKitErrorCode_IAPRestriction";
    std::string errorMessage = "IAP Restriction";
    OnErrorPurchaseResponse(
                      [invalidIdentifier UTF8String],
                      [invalidIdentifier lengthOfBytesUsingEncoding:NSUTF8StringEncoding],
                      StoreKitErrorCode_IAPRestriction,
                      errorMessage.c_str(),
                      errorMessage.length()
                      );
}

static std::string KeyPurchaseRecordMax = "KeyPMax";
static std::string KeyPurchaseRecord = "KeyP";

void StoreKitX::OnBeginPurchaseResponse(const char* storeItemID, const char* transactionID, const char* signature)
{
    if( IsProcessingPurchase() == false )
    {
        NSLog(@"OnBeginPurchasingResponse : why m_IsProcessingPurchase is 'false' ?  [%s]", storeItemID);
    }

    this->SetProcessingPurchase(true, storeItemID, transactionID);

    ///
    ///
    ///
    /// Comment by Allen: for FusePowered to track In-App Purchases
    ///
    /// TODO: registerInAppPurchase를 iOS 단에서 해주면, 유니티로 영수증 정보를 넘겨줄 필요가 없음!!!
    /// 1/ OnBeginPurchaseResponse에 파라미터 수정할 것!
    /// 2/ in-app 항목 요청은 한 번만 하도록 리스트화 할 것
    /// 3/ 유니티 단에서 호출하고있는 registerInAppPurchaseList/ registerInAppPurchase 를 호출 하지 않도록 수정할 것
    /// 4/ 어디서 하든 registerInAppPurchase를 호출하면 콜백으로 purchaseVerification 이 호출됨 -> 샌드박스 모드에서의 결과는 fusepowered가 알아서 해결해줄런지??
    /// 5/ OnProcessErrorPurchase 에서의 유니티로 돌아가는 경우는 언제인지?
    ///
    ///
    ///

    std::string receiptString = GetReceiptString();
    m_Delegate->OnBeginPurchaseResponse(storeItemID, transactionID, receiptString.c_str(), signature);
}
            
std::string StoreKitX::GetReceiptString()
{
    NSString *receiptString = nil;

    // Load the receipt from the app bundle.
    NSURL *receiptURL = [[NSBundle mainBundle] appStoreReceiptURL];
    NSData *receipt = [NSData dataWithContentsOfURL:receiptURL];
    
    if (!receipt) {
        /* No local receipt -- handle the error. */
        
        return "";
    }

    /* ... Send the receipt data to your server ... */
    receiptString = [receipt base64EncodedStringWithOptions:0];
    if (receiptString != nil) {
        //NSLog(@"%@", receiptString);
    }
    
    //NSLog(@"%s", [receiptString UTF8String]);
    return [receiptString UTF8String];
}

void StoreKitX::OnProcessErrorPurchase(const char* storeItemID, const char* transactionID)
{
    m_Delegate->OnProcessErrorPurchase(storeItemID, transactionID);
}

void StoreKitX::OnFinishPurchaseResponse(const char* storeItemID, const int lengthStoreItemID, const char* transactionID)
{
    if( IsProcessingPurchase() == false )
    {
        NSLog(@"OnBeginPurchasingResponse : why m_IsProcessingPurchase is 'false' ?  [%s]", storeItemID);
    }
    
    m_Delegate->OnFinishPurchaseResponse(storeItemID, lengthStoreItemID, transactionID);
    if( s_CurrentPaymentTransaction != nil )
    {
        [s_CurrentPaymentTransaction release];
    }
    s_CurrentPaymentTransaction = nil;
    
    this->SetProcessingPurchase(false, storeItemID, "0");

}

void StoreKitX::OnStoreItemInformationResponse(const StoreItemInformation storeItemInformation)
{
    if( IsProcessingPurchase() == true )
    {
//        ASSERT_DEBUG(IsProcessingPurchase() == false);
        return;
    }
    else
    {
        m_Delegate->OnStoreItemInformationResponse(storeItemInformation);
    }
}

void StoreKitX::OnStoreItemInformationResponse(const StoreItemList itemList)
{
    if( IsProcessingPurchase() == true )
    {
        //        ASSERT_DEBUG(IsProcessingPurchase() == false);
        return;
    }
    else
    {
        m_Delegate->OnStoreItemInformationResponse(itemList);
    }
}


void StoreKitX::OnErrorPurchaseResponse(const char* storeItemID, const int lengthStoreItemID, StoreKitErrorCode errorCode, const char* errorDescription, const int lengthErrorDescription)
{
    NSLog(@"OnErrorPurchaseResponse : [%s] [%s]", storeItemID, errorDescription);
    if( errorCode != StoreKitErrorCode_Success )
    {
        m_Delegate->OnErrorPurchaseResponse(storeItemID, lengthStoreItemID, errorCode, errorDescription, lengthErrorDescription);
        this->SetProcessingPurchase(false, "none", "0");

        if( errorCode == StoreKitErrorCode_InvalidProductIdentifier || errorCode == StoreKitErrorcode_TransactionFailed )
        {
            UIAlertView* alertView = [[UIAlertView alloc] initWithTitle:@"App Store" message:@"You can not access to App Store.\nYour payment request is canceled.\nNo Charge." delegate:nil cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
            [alertView release];
        }
        else if(errorCode == StoreKitErrorCode_IAPRestriction)
        {
            UIAlertView* alertView = [[UIAlertView alloc] initWithTitle:@"In-App Purchases" message:@"In-App Purchases are not currently allowed. Please enable them in Settings." delegate:nil cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
            [alertView release];
        }
    }
}

// StoreKitXDelegate Bridge
@implementation StoreKitXDelegateBridge
-(void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response
{
    if( StoreKitX::sharedStoreKitX()->getStoreItemRequestCount() == 0 && StoreKitX::sharedStoreKitX()->IsProcessingPurchase() == true )
    {   // Keep processing purchase-transaction
        
        if( response.invalidProductIdentifiers.count > 0 )
        {
                // NOTE :   StoreKitX only support single item purchase process.
                //          response.invalidProductIdentifiers.count must be 1
//            ASSERT_DEBUG(response.invalidProductIdentifiers.count == 1);
            for( NSString* invalidIdentifier in response.invalidProductIdentifiers )
            {
                std::string errorMessage = "invalid product identifier";
                StoreKitX::sharedStoreKitX()->OnErrorPurchaseResponse(
                                                                      [invalidIdentifier UTF8String],
                                                                      [invalidIdentifier lengthOfBytesUsingEncoding:NSUTF8StringEncoding],
                                                                      StoreKitErrorCode_InvalidProductIdentifier,
                                                                      errorMessage.c_str(),
                                                                      errorMessage.length()
                                                                      );
            }
            return;
        }
        
        SKProduct* product = [response.products firstObject];
        
        SKMutablePayment* payment = [SKMutablePayment paymentWithProduct:product];
        payment.quantity = 1;
        
        // For Detecting Irregular Activity ( Security )
        payment.applicationUsername = [StoreKitXDelegateBridge hashedValueForStoreUserKey:[NSString stringWithUTF8String: (StoreKitX::sharedStoreKitX()->GetAccountKey().c_str())]];
        // To Do : check applicationUsername matches with Username on receipt
        
        [[SKPaymentQueue defaultQueue] addPayment:payment];
    
        return;
    }

    StoreKitX::sharedStoreKitX()->decreaseStoreItemRequestCount();

    //
    // Retrieve products Information
    // No Popup needed, if errors occur!
    //
    if( response.invalidProductIdentifiers.count > 0 )
    {
        return;
    }

    SKProduct* product = [response.products firstObject];
    
    NSNumberFormatter* numberFormatter = [[NSNumberFormatter alloc] init];
    [numberFormatter setFormatterBehavior:NSNumberFormatterBehavior10_4];
    [numberFormatter setNumberStyle:NSNumberFormatterCurrencyStyle];
    [numberFormatter setLocale:product.priceLocale];
    NSString* formattedPrice = [numberFormatter stringFromNumber:product.price];
    
    StoreItemInformation information;
    information.m_StoreItemID = [[product productIdentifier] UTF8String];
    information.m_Price = [formattedPrice UTF8String];
    information.m_Currency = [[numberFormatter currencyCode] UTF8String];

    StoreKitX::sharedStoreKitX()->OnStoreItemInformationResponse(information);
}

+ (NSString *)hashedValueForStoreUserKey:(NSString*)storeUserKey
{
    const int HASH_SIZE = 32;
    unsigned char hashedChars[HASH_SIZE];
    const char *userKey = [storeUserKey UTF8String];
    size_t userKeyLen = strlen(userKey);
 
    // Confirm that the length of the user name is small enough
    // to be recast when calling the hash function.
    if (userKeyLen > UINT32_MAX) {
        NSLog(@"Account name too long to hash: %@\b", storeUserKey);
        return nil;
    }
    CC_SHA256(userKey, (CC_LONG)userKeyLen, hashedChars);
 
    // Convert the array of bytes into a string showing its hex representation.
    NSMutableString *userKeyHash = [[NSMutableString alloc] init];
    for (int i = 0; i < HASH_SIZE; i++) {
        // Add a dash every four bytes, for readability.
        if (i != 0 && i%4 == 0) {
            [userKeyHash appendString:@"-"];
        }
        [userKeyHash appendFormat:@"%02x", hashedChars[i]];
    }
 
    return userKeyHash;
}

- (void)saveReceiptToUserDefault:(SKPaymentTransaction*) transaction
{
    // Save Receipts to NSUserDefault (iOS only)
    #if USE_ICLOUD_STORAGE
        NSUbiquitousKeyValueStore *storage = [NSUbiquitousKeyValueStore defaultStore];
    #else
        NSUserDefaults *storage = [NSUserDefaults standardUserDefaults];
    #endif
    
    NSData *newReceipt;
    if (NSFoundationVersionNumber >= NSFoundationVersionNumber_iOS_7_0)
    {
        newReceipt = [NSData dataWithContentsOfURL:[[NSBundle mainBundle] appStoreReceiptURL]];
    }
    else
    {
        newReceipt = transaction.transactionReceipt;
    }
    
    if( newReceipt )
    {
        NSArray *savedReceipts = [storage arrayForKey:@"receipts"];
        if (!savedReceipts)
        {
            // Storing the first receipt
            [storage setObject:@[newReceipt] forKey:@"receipts"];
        }
        else
        {
            // Adding another receipt
            NSArray *updatedReceipts = [savedReceipts arrayByAddingObject:newReceipt];
            [storage setObject:updatedReceipts forKey:@"receipts"];
        }
        
        [storage synchronize];
    }
    else
    {
        NSLog(@"No Receipt on Transaction! %@\n", transaction.description);
    }
}


// SKPaymentTransactionStateObserver
- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions
{
    if(transactions != nil)
    {
        for (SKPaymentTransaction *transaction in transactions) {
            switch (transaction.transactionState) {
                    // Call the appropriate custom method.
                case SKPaymentTransactionStatePurchasing:
                {
                    // Update UI on Processing Purchasing and Wait
                    NSLog(@"Payment Transaction is on Purchasing %@\n", transaction.payment.productIdentifier);
                    
                    if(!StoreKitX::sharedStoreKitX()->IsBeginPurchase())
                    {//it's not begin purchase.
                        [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
                    }
                }
                    break;
                case SKPaymentTransactionStatePurchased:
                {
                    NSLog(@"Payment Transaction is Purchased %@\n", transaction.payment.productIdentifier);
                    
                    if (!StoreKitX::sharedStoreKitX()->IsProcessingErrorPurchase())
                    {
                        VerificationController* controller = [VerificationController sharedInstance];
                        [controller verifyPurchase:transaction completionHandler:^(BOOL success, SKPaymentTransaction* transaction)
                         {
                             [s_StoreKitXDelegateBridge onCompleteVerifyPurchase:success transaction:transaction];
                         }];
                    }
                    else
                    {
                        [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
                    }
                }
                    break;
                case SKPaymentTransactionStateFailed:
                {
                    NSLog(@"[StoreKitX] Payment Transaction Failed! %@\n", transaction.payment.productIdentifier);
                    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
                    StoreKitX::sharedStoreKitX()->OnErrorPurchaseResponse([transaction.payment.productIdentifier UTF8String], [transaction.payment.productIdentifier lengthOfBytesUsingEncoding:NSUTF8StringEncoding], StoreKitErrorcode_TransactionFailed, [transaction.error.description UTF8String], [transaction.error.description lengthOfBytesUsingEncoding:NSUTF8StringEncoding]);
                    
                }
                    break;
                case SKPaymentTransactionStateRestored:
                {
                    NSLog(@"Payment Transaction is Restored [%@]\n", transaction.payment.productIdentifier);
                    
                    VerificationController* controller = [VerificationController sharedInstance];
                    [controller verifyPurchase:transaction completionHandler:^(BOOL success, SKPaymentTransaction* transaction)
                     {
                         [s_StoreKitXDelegateBridge onCompleteVerifyPurchase:success transaction:transaction];
                     }];
                }
                    break;
                default:
                {
                    NSLog(@"Invalid Payment TransactionState");
                    //                NSAssert(NO, @"Invalid PaymentTransactionState");
                }
                    break;
            }
        }
    }
}

-(void)onCompleteVerifyPurchase:(BOOL)success transaction:(SKPaymentTransaction*)transaction
{
    if( success )
    {
        [self saveReceiptToUserDefault:transaction];

        
        ///
        /// Comment by Allen: for FusePowered to track In-App Purchases
        ///
        
//        [FuseSDK registerInAppPurchase:transaction];
        
     //[FuseSDK registerInAppPurchase:transaction.transactionReceipt TxState:transaction.transactionState Price:@"10.99" Currency:@"USD" ProductID:transaction.payment.productIdentifier];
        
        //FuseSDK_PaymentTransaction* paymentTransaction = [[FuseSDK_PaymentTransaction alloc] init];
        //[paymentTransaction.payment setIdentifier:"com.emmasse.fakeid"];
        //paymentTransaction.transactionState = 0;
       //paymentTransaction.transactionIdentifier = transaction.transactionIdentifier;
        //[FuseSDK registerInAppPurchase:(SKPaymentTransaction*)paymentTransaction];
        //[FuseSDK registerInAppPurchase:nil];
        
        if( s_CurrentPaymentTransaction != nil )
        {
            [s_CurrentPaymentTransaction release];
        }
        s_CurrentPaymentTransaction = transaction;
        [s_CurrentPaymentTransaction retain];
        
        NSDictionary *receiptDict       = [self dictionaryFromPlistData:transaction.transactionReceipt];
        NSString *signature = [receiptDict objectForKey:@"signature"];
        
        StoreKitX::sharedStoreKitX()->OnBeginPurchaseResponse([transaction.payment.productIdentifier UTF8String], [transaction.transactionIdentifier UTF8String], [signature UTF8String]);
    }
    else
    {
        if(transaction != nil)
        {
            NSLog(@"[StoreKitX] Restored Receipt Varification Failed! %@\n", transaction.payment.productIdentifier);
            StoreKitX::sharedStoreKitX()->OnErrorPurchaseResponse([transaction.payment.productIdentifier UTF8String], [transaction.payment.productIdentifier lengthOfBytesUsingEncoding:NSUTF8StringEncoding], StoreKitErrorCode_InvalidReceipt, "Restored Receipt Validation Failed", [transaction.error.description lengthOfBytesUsingEncoding:NSUTF8StringEncoding]);
            [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
        }
        else
        {
            NSLog(@"[StoreKitX] Transaction is null");
            StoreKitX::sharedStoreKitX()->ProcessErrorPurchase();
        }
    }
}

- (NSDictionary *)dictionaryFromPlistData:(NSData *)data
{
    NSError *error;
    NSDictionary *dictionaryParsed = [NSPropertyListSerialization propertyListWithData:data
                                                                               options:NSPropertyListImmutable
                                                                                format:nil
                                                                                 error:&error];
    if (!dictionaryParsed)
    {
        if (error)
        {
            NSLog(@"Error parsing plist");
        }
        return nil;
    }
    return dictionaryParsed;
}

- (void)paymentQueueRestoreCompletedTransactionsFinished:(SKPaymentQueue *)queue
{
    NSLog(@"PaymentQueue paymentQueueRestoreCompletedTransactionsFinished");

     NSMutableArray *purchasedItemIDs = [[NSMutableArray alloc] init];

    if(queue.transactions.count==0){

//        NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
//        NSArray *languages = [defaults objectForKey:@"AppleLanguages"];
//        NSString *currentLanguage = [languages objectAtIndex:0];
//        
//        NSString *failMessage;
//        //Language process
//        if ([currentLanguage isEqualToString:@"ko"]) {
//            failMessage = @"구매 기록이 없습니다.";
//        }else{
//            failMessage = @"There is no record of your purchase.";
//        }
//        
//        UIAlertView *resultView = [[UIAlertView alloc] initWithTitle:@"Failed"
//                                                             message:failMessage
//                                                            delegate:self
//                                                   cancelButtonTitle:nil
//                                                   otherButtonTitles:@"OK", nil];
//        [resultView show];
    }
    
    for (SKPaymentTransaction *transaction in queue.transactions)
    {
        NSString *productID = transaction.payment.productIdentifier;
        [purchasedItemIDs addObject:productID];
        NSLog (@"product id is %@" , productID);
        // here put an if/then statement to write files based on previously purchased items
        // example if ([productID isequaltostring: @"youruniqueproductidentifier]){write files} else { nslog sorry}
        
        if([productID isEqualToString:productID])
        {
            if([productID isEqualToString:@"com.sally.costume"])
            {
                UnitySendMessage("Storekit_Sally", "Callback_StorekitSally_RestorePurchase", "");
                
                //재구입확인dh
                NSLog(@"already buy");
            }
            else
                UnitySendMessage("Storekit_Sally", "Callback_StorekitSally_RestorePurchaseError", "");
        }

        
        [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
    }
}

-(void)paymentQueue:(SKPaymentQueue *)queue restoreCompletedTransactionsFailedWithError:(NSError *)error
{
    NSLog(@"PaymentQueue RestoreCompletedTransactionsFailedWithError : %s", [error.description UTF8String]);
    UnitySendMessage("Storekit_Sally", "Callback_StorekitSally_RestorePurchaseError", "");
}

-(void)paymentQueue:(SKPaymentQueue *)queue removedTransactions:(NSArray *)transactions
{
    for(SKPaymentTransaction* transaction in transactions)
    {
        NSLog(@"PaymentQueue RemovedTransactions : [%s] [%s]", [transaction.payment.productIdentifier UTF8String], [transaction.description UTF8String]);
    }
}

void StoreKitX::OnRestoreItem()
{
    NetworkStatus netStatus = [[Reachability reachabilityForInternetConnection] currentReachabilityStatus];
    if(netStatus == NotReachable)
    {
        // Error popup
        UIAlertView* alertView = [[UIAlertView alloc] initWithTitle:@"In-App Purchases" message:@"Please connect to the Internet." delegate:nil cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
        [alertView release];
        
        UnitySendMessage("Storekit_Sally", "Callback_StorekitSally_RestorePurchaseError", "");
        NSLog(@"Internet connection error");
    }else{
        [[SKPaymentQueue defaultQueue] restoreCompletedTransactions];
    }
}

@end


