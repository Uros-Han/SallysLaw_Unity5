//
//  Storekit_Sally.h
//  Unity-iPhone
//
//  Created by Sinhyub on 9/5/14.
//
//

#ifndef Unity_iPhone_Storekit_Sally_h
#define Unity_iPhone_Storekit_Sally_h

#ifdef __cplusplus
#include <string>
#include <vector>
#include "StoreKitX.h"

enum StoreItemType
{
    StoreItemType_None = -1,
    StoreItemType_Month,
    StoreItemType_Year
};

class StorekitSally : public StoreKitXDelegate
{
private:
    static StorekitSally*   _SingletonInstance;
    
    StorekitSally();
public:
    virtual ~StorekitSally();
    
    static StorekitSally& GetInstance();
    bool                Initialize(std::string userAccountID);
    void                ProcessErrorPurchase();
    
    
    virtual void        OnBeginPurchaseResponse(const char* storeItemID, const char* transactionID, const char* receipt,const char* signature);
//    virtual void        OnBeginPurchaseResponse(const char* storeItemID, const char* transactionID);
//    virtual void        OnProcessErrorPurchase(const char* storeItemID, const char* transactionID, const char* receipt);
    virtual void        OnProcessErrorPurchase(const char* storeItemID, const char* transactionID);
    virtual void        OnFinishPurchaseResponse(const char* storeItemID, const int lengthStoreItemID,const char* transactionID);
    virtual void        OnStoreItemInformationResponse(const StoreItemInformation storeItemInformation);
    virtual void        OnStoreItemInformationResponse(const StoreItemList itemList);
    
    virtual void        OnErrorPurchaseResponse(const char* storeItemID, const int lengthStoreItemID, StoreKitErrorCode errorCode, const char* errorDescription, const int lengthErrorDescription);
    virtual void        OnErrorPurchaseBegin();
};

#endif // __cplusplus

extern "C"
{
    bool StorekitSally_Initialize(char* userAccontID);
    void StorekitSally_ProcessErrorPurchase();
    int  StorekitSally_BeginPurchase(char* storeItemID);
    bool StorekitSally_FinishPurchase(char* storeItemID);
    bool StorekitSally_GetStoreItemInformation(char* storeItemID);
    void StorekitSally_RestoreItem();
}

#endif
