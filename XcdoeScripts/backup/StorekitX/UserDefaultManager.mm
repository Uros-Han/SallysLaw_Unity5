//
//  UserDefaultManager.mm
//  Unity-iPhone
//
//  Created by Sinhyub on 9/5/14.
//
//

#import "UserDefaultManager.h"

UserDefaultManager* UserDefaultManager::_SingletonInstance = NULL;

#if USE_ICLOUD_STORAGE
#define UserDefaultContainer NSUbiquitousKeyValueStore
#else
#define UserDefaultContainer NSUserDefaults
#endif


UserDefaultManager::UserDefaultManager()
{

}

UserDefaultManager::~UserDefaultManager()
{

}

UserDefaultManager& UserDefaultManager::GetInstance()
{
    if( _SingletonInstance == NULL )
    {
        _SingletonInstance = new UserDefaultManager();
    }
    
    return *_SingletonInstance;
}


bool UserDefaultManager::GetBoolForKey(const char* key)
{
    return GetBoolForKey(key, false);
}

bool UserDefaultManager::GetBoolForKey(const char* key, bool defaultValue)
{
    bool ret = defaultValue;
    
    NSNumber *value = [[UserDefaultContainer standardUserDefaults] objectForKey:[NSString stringWithUTF8String:key]];
    if (value)
    {
        ret = [value boolValue];
    }
    
    return ret;
}

int UserDefaultManager::GetIntegerForKey(const char* key)
{
    return GetIntegerForKey(key, 0);
}

int UserDefaultManager::GetIntegerForKey(const char* key, int defaultValue)
{
    int ret = defaultValue;
    
    NSNumber *value = [[UserDefaultContainer standardUserDefaults] objectForKey:[NSString stringWithUTF8String:key]];
    if (value)
    {
        ret = [value intValue];
    }
    
    return ret;
}

float UserDefaultManager::GetFloatForKey(const char* key)
{
    return GetFloatForKey(key, 0);
}

float UserDefaultManager::GetFloatForKey(const char* key, float defaultValue)
{
    float ret = defaultValue;
    
    NSNumber *value = [[UserDefaultContainer standardUserDefaults] objectForKey:[NSString stringWithUTF8String:key]];
    if (value)
    {
        ret = [value floatValue];
    }
    
    return ret;
}

double  UserDefaultManager::GetDoubleForKey(const char* key)
{
    return GetDoubleForKey(key, 0);
}

double UserDefaultManager::GetDoubleForKey(const char* key, double defaultValue)
{
	double ret = defaultValue;
    
    NSNumber *value = [[UserDefaultContainer standardUserDefaults] objectForKey:[NSString stringWithUTF8String:key]];
    if (value)
    {
        ret = [value doubleValue];
    }
    
    return ret;
}

std::string UserDefaultManager::GetStringForKey(const char* key)
{
    return GetStringForKey(key, "");
}

std::string UserDefaultManager::GetStringForKey(const char* key, const std::string & defaultValue)
{
    NSString *str = [[UserDefaultContainer standardUserDefaults] stringForKey:[NSString stringWithUTF8String:key]];
    if (! str)
    {
        return defaultValue;
    }
    else
    {
        return [str UTF8String];
    }
}

void UserDefaultManager::SetBoolForKey(const char* key, bool value)
{
    [[UserDefaultContainer standardUserDefaults] setObject:[NSNumber numberWithBool:value] forKey:[NSString stringWithUTF8String:key]];
}

void UserDefaultManager::SetIntegerForKey(const char* key, int value)
{
    [[UserDefaultContainer standardUserDefaults] setObject:[NSNumber numberWithInt:value] forKey:[NSString stringWithUTF8String:key]];
}

void UserDefaultManager::SetFloatForKey(const char* key, float value)
{
    [[UserDefaultContainer standardUserDefaults] setObject:[NSNumber numberWithFloat:value] forKey:[NSString stringWithUTF8String:key]];
}

void UserDefaultManager::SetDoubleForKey(const char* key, double value)
{
    [[UserDefaultContainer standardUserDefaults] setObject:[NSNumber numberWithDouble:value] forKey:[NSString stringWithUTF8String:key]];
}

void UserDefaultManager::SetStringForKey(const char* key, const std::string & value)
{
    [[UserDefaultContainer standardUserDefaults] setObject:[NSString stringWithUTF8String:value.c_str()] forKey:[NSString stringWithUTF8String:key]];
}

void UserDefaultManager::Flush()
{
    [[UserDefaultContainer standardUserDefaults] synchronize];
}

