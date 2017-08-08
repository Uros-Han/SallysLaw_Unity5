//
//  StoreKitX.h
//  BeautyCrush
//
//  Created by Sinhyub Kim on 3/13/14.
//
//

///////////////////////////////////////////

#include <vector>
#include <string>

#ifndef BeautyCrush_StoreKitX_h
#define BeautyCrush_StoreKitX_h

enum StoreKitErrorCode : int
{
    // Dont Change order of this error codes.
    // This Codes are matched with android version storekit error codes (java)
    StoreKitErrorCode_None = -1,
    StoreKitErrorCode_Success = 0,
    StoreKitErrorCode_InvalidProductIdentifier,
    StoreKitErrorcode_TransactionFailed,
    StoreKitErrorCode_InvalidReceipt_NoReceiptOnTransaction,
    StoreKitErrorCode_NoCurrentPurchaseOnFinishPurchase,
    StoreKitErrorCode_InvalidReceipt,       // Receipt Validation Failed
    StoreKitErrorCode_IAPRestriction,   // IAP Restriction,
    StoreKitErrorCode_Max,
};

enum StoreItemCategory : int
{
    StoreItemCategory_None = -1,
    StoreItemCategory_Consumable,
    StoreItemCategory_NonConsumable,
    StoreItemCategory_Subscription,
    StoreItemCategory_Max,
};

typedef struct
{
public:
    std::string     m_StoreItemID;
    std::string     m_Price;
    std::string     m_Currency;
} StoreItemInformation ;

typedef std::vector<StoreItemInformation> StoreItemList;

class StoreKitXDelegate {
public:
    virtual ~StoreKitXDelegate() {}
    
    virtual void        OnBeginPurchaseResponse(const char* storeItemID, const char* transactionID, const char* receipt, const char* signature) = 0;
//    virtual void        OnBeginPurchaseResponse(const char* storeItemID, const char* transactionID) = 0;
//    virtual void        OnProcessErrorPurchase(const char* storeItemID, const char* transactionID, const char* receipt) = 0;
    virtual void        OnProcessErrorPurchase(const char* storeItemID, const char* transactionID) = 0;
    virtual void        OnFinishPurchaseResponse(const char* storeItemID, const int lengthStoreItemID,const char* transactionID) = 0;
    virtual void        OnStoreItemInformationResponse(const StoreItemInformation storeItemInformation) = 0;
    virtual void        OnStoreItemInformationResponse(const StoreItemList itemList) = 0;
    
    virtual void        OnErrorPurchaseResponse(const char* storeItemID, const int lengthStoreItemID, StoreKitErrorCode errorCode, const char* errorDescription, const int lengthErrorDescription) = 0;
    virtual void        OnErrorPurchaseBegin() = 0;
};


class StoreKitX
{
private:
    static StoreKitX*       _instance;
    StoreKitXDelegate*      m_Delegate;
    
    std::string             m_AccountKey;
    bool                    m_IsProcessingPurchase;
    bool                    m_IsProcessingErrorPurchase;
    bool                    m_IsBeginPurchase;
    int                     m_StoreItemRequestCount;

private:
    StoreKitX();
public:
    virtual ~StoreKitX();
    static StoreKitX*   sharedStoreKitX();
    
    void                SetProcessingPurchase(bool isProcessingPurchase, std::string storeItemID, std::string transactionID);
    bool                IsProcessingPurchase()  {return m_IsProcessingPurchase;}
    bool                IsProcessingErrorPurchase()  {return m_IsProcessingErrorPurchase;}
    bool                IsBeginPurchase()   { return m_IsBeginPurchase;}
    int                 getStoreItemRequestCount()  {return m_StoreItemRequestCount;}
    void                decreaseStoreItemRequestCount()  {m_StoreItemRequestCount--;}
    
public:
    bool                Initialize(StoreKitXDelegate* delegate, std::string accountKey);
    void                ProcessErrorPurchase();
    std::string         GetAccountKey() {   return m_AccountKey; }
private:
    void                SetDelegate(StoreKitXDelegate* delegate) { m_Delegate = delegate; }
private:
    StoreKitXDelegate*  GetDelegate(){  return m_Delegate;}
    void                showCannotMakePurchasePopup();
    std::string         GetReceiptString();
  
  
public:
    int                BeginPurchase(StoreItemCategory storeItemCategory, std::string storeItemID);//  const char* storeItemID, const int lengthStoreItemID);
    bool                FinishPurchase(StoreItemCategory storeItemCategory, std::string storeItemID);// const char* storeItemID, const int lengthStoreItemID);
    bool                GetStoreItemInformation(std::string storeItemID);
    
    void                OnBeginPurchaseResponse(const char* storeItemID, const char* transactionID, const char* signature);
    void                OnProcessErrorPurchase(const char* storeItemID, const char* transactionID);
    void                OnFinishPurchaseResponse(const char* storeItemID, const int lengthStoreItemID,const char* transcationID);
    void                OnStoreItemInformationResponse(const StoreItemInformation storeItemInformation);
    void                OnStoreItemInformationResponse(const StoreItemList itemList);
    void                OnErrorPurchaseBegin();
    void                OnErrorPurchaseResponse(const char* storeItemID, const int lengthStoreItemID, StoreKitErrorCode errorCode, const char* errorDescription, const int lengthErrorDescription);
    void                OnRestoreItem();
};

#endif


