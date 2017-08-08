//
//  StorekitSally.mm
//  Unity-iPhone
//
//  Created by Sinhyub on 9/5/14.
//
//

#include "Storekit_Sally.h"
#include "JSONKit.h"
#import <AdSupport/ASIdentifierManager.h>

StorekitSally* StorekitSally::_SingletonInstance = NULL;

StorekitSally::StorekitSally()
{
    
}

StorekitSally::~StorekitSally()
{
    
}

StorekitSally& StorekitSally::GetInstance()
{
    if( _SingletonInstance == NULL )
    {
        _SingletonInstance = new StorekitSally();
    }
    
    return *_SingletonInstance;
}

bool StorekitSally::Initialize(std::string userAccountID)
{
    #warning "Get UserAccount Key and Pass to Initialize Method
    return StoreKitX::sharedStoreKitX()->Initialize(this, userAccountID);
}

void StorekitSally::ProcessErrorPurchase()
{
    StoreKitX::sharedStoreKitX()->ProcessErrorPurchase();
}

void StorekitSally::OnBeginPurchaseResponse(const char* storeItemID, const char* transactionID, const char* receipt, const char* signature)
//void StorekitSally::OnBeginPurchaseResponse(const char* storeItemID, const char* transactionID)
{
//    const std::string storeItemIDString(storeItemID);
//    UnitySendMessage("Storekit_Sally", "Callback_StorekitSally_OnBeginPurchaseResponse", storeItemIDString.c_str());

    NSMutableDictionary* storeItemInformationDic = [[NSMutableDictionary alloc] init];
    [storeItemInformationDic setObject:[NSString stringWithUTF8String:storeItemID] forKey:@"storeItemID"];
    [storeItemInformationDic setObject:[NSString stringWithUTF8String:transactionID] forKey:@"transactionID"];
    NSString *idfaString = [[[ASIdentifierManager sharedManager] advertisingIdentifier] UUIDString];
    [storeItemInformationDic setObject:[NSString stringWithUTF8String:[idfaString UTF8String]] forKey:@"idfaID"];
    [storeItemInformationDic setObject:[NSString stringWithUTF8String:receipt] forKey:@"receipt"];
    [storeItemInformationDic setObject:[NSString stringWithUTF8String:signature] forKey:@"signature"];
    
    NSError* error = nil;
    NSString* json = [storeItemInformationDic JSONStringWithOptions:JKSerializeOptionPretty|JKSerializeOptionEscapeUnicode|JKSerializeOptionEscapeForwardSlashes error:&error];
    if( error )
    {
        json = @"{}";
    }
    
   // NSLog(@"%@", json);

    UnitySendMessage("Storekit_Sally", "Callback_StorekitSally_OnBeginPurchaseResponse", [json UTF8String]);

}

//void StorekitSally::OnProcessErrorPurchase(const char* storeItemID, const char* transactionID, const char* receipt)
void StorekitSally::OnProcessErrorPurchase(const char* storeItemID, const char* transactionID)
{
    NSMutableDictionary* storeItemInformationDic = [[NSMutableDictionary alloc] init];
    [storeItemInformationDic setObject:[NSString stringWithUTF8String:storeItemID] forKey:@"storeItemID"];
    [storeItemInformationDic setObject:[NSString stringWithUTF8String:transactionID] forKey:@"transactionID"];
//    [storeItemInformationDic setObject:[NSString stringWithUTF8String:receipt] forKey:@"receipt"];
    
    NSError* error = nil;
    NSString* json = [storeItemInformationDic JSONStringWithOptions:JKSerializeOptionPretty|JKSerializeOptionEscapeUnicode|JKSerializeOptionEscapeForwardSlashes error:&error];
    if( error )
    {
        json = @"{}";
    }
    UnitySendMessage("Storekit_Sally", "Callback_StorekitSally_OnProcessErrorPurchase", [json UTF8String]);
}

void StorekitSally::OnFinishPurchaseResponse(const char* storeItemID, const int lengthStoreItemID,const char* transactionID)
{
    //const std::string storeItemIDString(storeItemID);
    //UnitySendMessage("Storekit_Sally", "Callback_StorekitSally_OnFinishPurchaseResponse", storeItemIDString.c_str());
    NSMutableDictionary* storeItemInformationDic = [[NSMutableDictionary alloc] init];
    [storeItemInformationDic setObject:[NSString stringWithUTF8String:storeItemID] forKey:@"storeItemID"];
    [storeItemInformationDic setObject:[NSString stringWithUTF8String:transactionID] forKey:@"transactionID"];
    
    NSString *idfaString = [[[ASIdentifierManager sharedManager] advertisingIdentifier] UUIDString];
   [storeItemInformationDic setObject:[NSString stringWithUTF8String:[idfaString UTF8String]] forKey:@"idfaID"];
    
    NSError* error = nil;
    NSString* json = [storeItemInformationDic JSONStringWithOptions:JKSerializeOptionPretty|JKSerializeOptionEscapeUnicode|JKSerializeOptionEscapeForwardSlashes error:&error];
    if( error )
    {
        json = @"{}";
    }
    
    UnitySendMessage("Storekit_Sally", "Callback_StorekitSally_OnFinishPurchaseResponse", [json UTF8String]);
}

void StorekitSally::OnStoreItemInformationResponse(const StoreItemInformation storeItemInformation)
{
    NSMutableDictionary* storeItemInformationDic = [[NSMutableDictionary alloc] init];
    [storeItemInformationDic setObject:[NSString stringWithUTF8String:storeItemInformation.m_StoreItemID.c_str()] forKey:@"storeItemID"];
    [storeItemInformationDic setObject:[NSString stringWithUTF8String:storeItemInformation.m_Price.c_str()] forKey:@"price"];
    [storeItemInformationDic setObject:[NSString stringWithUTF8String:storeItemInformation.m_Currency.c_str()] forKey:@"currency"];
    
    NSError* error = nil;
    NSString* json = [storeItemInformationDic JSONStringWithOptions:JKSerializeOptionPretty|JKSerializeOptionEscapeUnicode|JKSerializeOptionEscapeForwardSlashes error:&error];
    if( error )
    {
        json = @"{}";
    }

    
    
    UnitySendMessage("Storekit_Sally", "Callback_StorekitSally_OnStoreItemInformationResponse", [json UTF8String]);
}

void StorekitSally::OnStoreItemInformationResponse( StoreItemList itemList)
{
    NSError* error = nil;
    
    NSMutableDictionary* itemListDic = [[NSMutableDictionary alloc] init];
    {
        NSMutableArray *itemListArray = [[NSMutableArray alloc] init];
        for (int i=0; i<itemList.size(); i++) {
            NSMutableDictionary* storeItemInformationDic = [[NSMutableDictionary alloc] init];
            [storeItemInformationDic setObject:[NSString stringWithUTF8String:itemList.at(i).m_StoreItemID.c_str()] forKey:@"storeItemID"];
            [storeItemInformationDic setObject:[NSString stringWithUTF8String:itemList.at(i).m_Price.c_str()] forKey:@"price"];
            [storeItemInformationDic setObject:[NSString stringWithUTF8String:itemList.at(i).m_Currency.c_str()] forKey:@"currency"];
            
            
            [itemListArray addObject:storeItemInformationDic];
        }
        
        [itemListDic setObject:itemListArray forKey:@"id"];
    }
    
    NSString* json = [itemListDic JSONStringWithOptions:JKSerializeOptionPretty|JKSerializeOptionEscapeUnicode|JKSerializeOptionEscapeForwardSlashes error:&error];
    if( error )
    {
        json = @"{}";
    }
    
    //NSLog(@"%@", json);
    
    UnitySendMessage("Storekit_Sally", "Callback_StorekitSally_OnStoreItemInformationResponse", [json UTF8String]);
}

void StorekitSally::OnErrorPurchaseResponse(const char* storeItemID, const int lengthStoreItemID, StoreKitErrorCode errorCode, const char* errorDescription, const int lengthErrorDescription)
{
    if(storeItemID != NULL)
    {
        const std::string storeItemIDString(storeItemID);
        
        NSMutableDictionary* errorInformationDic = [[NSMutableDictionary alloc] init];
        [errorInformationDic setObject:[NSString stringWithUTF8String:storeItemIDString.c_str()] forKey:@"storeItemID"];
        if(errorDescription != NULL)
        {
            const std::string errorDescriptionString(errorDescription);
            [errorInformationDic setObject:[NSString stringWithUTF8String:errorDescriptionString.c_str()] forKey:@"errorDescription"];
        }
        [errorInformationDic setObject:[NSString stringWithFormat:@"%d",errorCode] forKey:@"errorCode"];
        
        NSError* error = nil;
        NSString* json = [errorInformationDic JSONStringWithOptions:JKSerializeOptionPretty|JKSerializeOptionEscapeUnicode|JKSerializeOptionEscapeForwardSlashes error:&error];
        if( error )
        {
            json = @"{}";
        }
        
        UnitySendMessage("Storekit_Sally", "Callback_StorekitSally_OnErrorPurchaseResponse", [json UTF8String]);
    }
}

void StorekitSally::OnErrorPurchaseBegin()
{
    UnitySendMessage("Storekit_Sally", "Callback_StorekitSally_OnFinishPurchaseResponse", "");
}

extern "C"
{
    bool StorekitSally_Initialize(char* userAccontID)
    {
        std::string userAccountIDString(userAccontID);
        return StorekitSally::GetInstance().Initialize(userAccountIDString);
    }
    
    void StorekitSally_ProcessErrorPurchase()
    {
        StorekitSally::GetInstance().ProcessErrorPurchase();
    }
    
    int StorekitSally_BeginPurchase(char* storeItemID)
    {
        NSString* storeItemIDString = [NSString stringWithUTF8String:storeItemID];
        return StoreKitX::sharedStoreKitX()->BeginPurchase(StoreItemCategory_Consumable, [storeItemIDString UTF8String]);//test_gem_10
    }

    bool StorekitSally_FinishPurchase(char* storeItemID)
    {
        NSString* storeItemIDString = [NSString stringWithUTF8String:storeItemID];
        return StoreKitX::sharedStoreKitX()->FinishPurchase(StoreItemCategory_Consumable, [storeItemIDString UTF8String]);
    }

    bool StorekitSally_GetStoreItemInformation(char* storeItemID)
    {
        NSString* storeItemIDString = [NSString stringWithUTF8String:storeItemID];
        return StoreKitX::sharedStoreKitX()->GetStoreItemInformation([storeItemIDString UTF8String]);
    }
    
    void StorekitSally_RestoreItem()
    {
        StoreKitX::sharedStoreKitX()->OnRestoreItem();
    }

}
