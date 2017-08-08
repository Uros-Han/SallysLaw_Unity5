//
//  UserDefaultManager.h
//  Unity-iPhone
//
//  Created by Sinhyub on 9/5/14.
//
//

#include <string>
//
//@interface UserDefaultManager : NSObject
//
//@end

class UserDefaultManager
{
private:
    static UserDefaultManager*      _SingletonInstance;
    UserDefaultManager();
    
public:
    ~UserDefaultManager();
    static UserDefaultManager& GetInstance();
    
    bool        GetBoolForKey(const char* key);
    bool        GetBoolForKey(const char* key, bool defaultValue);

    int         GetIntegerForKey(const char* key);
    int         GetIntegerForKey(const char* key, int defaultValue);

    float       GetFloatForKey(const char* key);
    float       GetFloatForKey(const char* key, float defaultValue);

    double      GetDoubleForKey(const char* key);
    double      GetDoubleForKey(const char* key, double defaultValue);

    std::string GetStringForKey(const char* key);
    std::string GetStringForKey(const char* key, const std::string & defaultValue);
    

    void        SetBoolForKey(const char* key, bool value);
    void        SetIntegerForKey(const char* key, int value);

    void        SetFloatForKey(const char* key, float value);
    void        SetDoubleForKey(const char* key, double value);

    void        SetStringForKey(const char* key, const std::string & value);
    void        Flush();
    
    
    
};