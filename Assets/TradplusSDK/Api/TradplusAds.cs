using System;
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using TradplusSDK.Unity;
#elif UNITY_IOS
using TradplusSDK.iOS;
#elif UNITY_ANDROID
using TradplusSDK.Android;
#else
using TradplusSDK.Unity;
#endif

using System.Collections.Generic;

namespace TradplusSDK.Api
{
    public class TPAds :
#if UNITY_EDITOR
    TPSdkUnityAds
#elif UNITY_IOS
    TradplusAdsiOS
#elif UNITY_ANDROID
     TradplusAdsAndroid
#else
    TPSdkUnityAds
#endif
    { }

    public enum TPPlatformID
    {
        Meta = 1,
        Admob = 2,
        AdColony = 4,
        UnityAds = 5,
        Tapjoy = 6,
        Liftoff = 7,
        AppLovin = 9,
        IronSource = 10,
        Chartboost = 15,
        TencentAds = 16,//优量汇
        CSJ = 17,//穿山甲
        Mintegral = 18,
        Pangle = 19,
        KuaishouAds = 20,
        Sigmob = 21,
        Inmobi = 23,
        Fyber = 24,
        YouDao = 25,
        CrossPromotion = 27,//交叉推广
        StartIO = 28,
        Helium = 30,
        Maio = 31,
        Criteo = 32,
        MyTarget = 33,
        Ogury = 34,
        AppNext = 35,
        Kidoz = 37,
        Smaato = 38,
        ADX = 40,
        HuaWei = 41,
        Baidu = 43,//百度
        Klevin = 44,//游可赢
        A4G = 45,
        Mimo = 46,//米盟
        SuperAwesome = 47,
        GoogleAdManager = 48,
        Yandex = 50,
        Verve = 53,
        ZMaticoo = 55,
        ReklamUp = 56,
        Bigo = 57,
        Beizi = 58,
        TapTap = 63,
        ONEMOB = 60,
        PremiumAds = 64,
        GreedyGame = 67,
        AlgoriX = 68,
        BeesAds = 69,
        Amazon = 70,
        MangoX = 71,
        Sailoff = 72,
        TanX = 73,//阿里妈妈
        TaurusX = 74,
        KwaiAds = 75,
        Columbus = 76,
        YSO = 77,
        VivoAds = 78,
        OppoAds = 79,
        HONOR = 80,
    }

    public enum TPPAGPAConsentType
    {
        NoConsent = 0,   ///< User doesn't grant consent
        Consent = 1,   ///< User has granted the consent
    }

    public class TPPlatformLimit
    {
        ArrayList list;
        public TPPlatformLimit()
        {
            list = new ArrayList();
        }

        public void SetLimit(int platform, int num)
        {
            list.Add(new Dictionary<string, int>()
            {
               {"platform",platform },{"num",num}

            });
        }
        public Array GetList()
        {
            return list.ToArray();
        }
    }

    public class TTDUID2Extra
    {
        ///<summary>
        ///subscriptionID，必填
        ///</summary>
        public String subscriptionID;

        ///<summary>
        ///serverPublicKey,必填
        ///</summary>
        public String serverPublicKey;

        ///<summary>
        ///email,emailHash,phone,phoneHash需至少设置一个
        ///</summary>
        public String email;
        public String emailHash;
        public String phone;
        public String phoneHash;

        ///<summary>
        ///appName,选填
        ///</summary>
        public String appName;

        ///<summary>
        ///customURLString,选填
        ///</summary>
        public String customURLString;

        public bool isTestMode;
    }

    public class TradplusAds
    {
        public static string PluginVersion = "1.3.5";

        private static TradplusAds _instance;

        private bool didAddGlobalAdImpressionListener;
        Action<Dictionary<string, object>> onGlobalAdImpression;

        public static TradplusAds Instance()
        {
            if (_instance == null)
            {
                _instance = new TradplusAds();
            }
            return _instance;
        }

        ///<summary>
		///设置Pangle是否投放广告（PangleSDK V7.1+）
        ///</summary>
        public void SetPAConsent(TPPAGPAConsentType consentType)
        {
            TPAds.Instance().SetPAConsent(consentType);
        }

        ///<summary>
		///设置自定义测试ID
        ///</summary>
        public void SetCustomTestID(string customTestID)
        {
            TPAds.Instance().SetCustomTestID(customTestID);
        }

        ///<summary>
        ///设置指定广告平台的频次限制次数
        ///可配合TPPlatformLimit使用
        ///</summary>
        public void setPlatformLimit(Array list)
        {
            TPAds.Instance().setPlatformLimit(list);
        }


        public void setForbidNetworkIdList(Array list)
        {
            TPAds.Instance().setForbidNetworkIdList(list);
        }

        ///<summary>
        ///启动UID2
        ///</summary>
        public void startUID2(TTDUID2Extra extra)
        {
            if (extra != null)
            {
                if (extra.subscriptionID == null)
                {
                    Debug.LogError("UID2Extra.subscriptionID can not be null");
                    return;
                }
                if (extra.serverPublicKey == null)
                {
                    Debug.LogError("UID2Extra.serverPublicKey can not be null");
                    return;
                }
                if (extra.email == null
                    && extra.emailHash == null
                    && extra.phone == null
                    && extra.phoneHash == null)
                {
                    Debug.LogError("UID2Extra email,emailHash,phone,phoneHash set at least one");
                    return;
                }
                TPAds.Instance().startUID2(extra);
            }
            else
            {
                Debug.LogError("UID2Extra can not be null");
            }
        }
        ///<summary>
        ///重置UID2
        ///</summary>
        public void resetSetting()
        {
            TPAds.Instance().resetSetting();
        }

        ///<summary>
        ///查询当前地区，此接口一般在初始化前调用来获取当前设备的地区状态。开发者可根据返回数据针对地区情况来设置各隐私权限。
        ///使用时需要设置回调 OnCurrentAreaSuccess & OnCurrentAreaFailed 来获取查询状态。
        ///OnCurrentAreaSuccess 返回的地区数据包括： bool isEu 是否欧洲, bool isCn 是否中国, bool isCa 是否加州
        ///OnCurrentAreaFailed 时开发者需要自行查询或处理，设置各隐私权限。
        ///</summary>
        public void CheckCurrentArea()
        {
            TPAds.Instance().CheckCurrentArea();
        }

        ///<summary>
        ///初始化SDK
        ///</summary>
        public void InitSDK(string appId)
        {
            TPAds.Instance().InitSDK(appId);
        }

        ///<summary>
        ///设置流量分组等自定数据，需要在初始化前设置
        ///</summary>
        public void SetCustomMap(Dictionary<string, string> customMap)
        {
            TPAds.Instance().SetCustomMap(customMap);
        }

        ///<summary>
        ///设置预配置 adUnitId 广告位ID
		///config 从TP后台导出的预配置（base64格式）
        ///</summary>
        public void SetDefaultConfig(string adUnitId, string config)
        {
            TPAds.Instance().SetDefaultConfig(adUnitId,config);
        }

        ///<summary>
        ///设置Setting级别数据
        ///</summary>
        public void SetSettingDataParam(Dictionary<string, object> settingMap)
        {
            TPAds.Instance().SetSettingDataParam(settingMap);
        }

        ///<summary>
        ///获取 TradplusSDK 版本号
        ///</summary>
        public string Version()
        {
            return TPAds.Instance().Version();
        }

        ///<summary>
        ///是否在欧盟地区 此接口需要在初始化成功后调用
        ///</summary>
        public bool IsEUTraffic()
        {
            return TPAds.Instance().IsEUTraffic();
        }

        ///<summary>
        ///是否在加州地区 此接口需要在初始化成功后调用
        ///</summary>
        public bool IsCalifornia()
        {
            return TPAds.Instance().IsCalifornia();
        }

        ///<summary>
        ///设置 GDPR等级 是否允许数据上报: ture 设备数据允许上报, false 设备数据不允许上报
        ///</summary>
        public void SetGDPRDataCollection(bool canDataCollection)
        {
            TPAds.Instance().SetGDPRDataCollection(canDataCollection);
        }

        ///<summary>
        ///获取当前 GDPR等级：  0 允许上报 , 1 不允许上报, 2 未设置
        ///</summary>
        public int GetGDPRDataCollection()
        {
            return TPAds.Instance().GetGDPRDataCollection();
        }

        ///<summary>
        ///设置 LGPD等级 是否允许数据上报: ture 设备数据允许上报, false 设备数据不允许上报
        ///</summary>
        public void SetLGPDConsent(bool consent)
        {
            TPAds.Instance().SetLGPDConsent(consent);
        }

        ///<summary>
        ///获取当前 LGPD等级： 0 允许上报 , 1 不允许上报 2 未设置
        ///</summary>
        public int GetLGPDConsent()
        {
            return TPAds.Instance().GetLGPDConsent();
        }

        ///<summary>
        ///设置 CCPA等级 是否允许数据上报: ture 加州用户接受上报数据, false 加州用户均不上报数据
        ///</summary>
        public void SetCCPADoNotSell(bool canDataCollection)
        {
            TPAds.Instance().SetCCPADoNotSell(canDataCollection);
        }

        ///<summary>
        ///获取当前 CCPA等级： 0 允许上报 , 1 不允许上报, 2 未设置
        ///</summary>
        public int GetCCPADoNotSell()
        {
            return TPAds.Instance().GetCCPADoNotSell();
        }

        ///<summary>
        ///设置 COPPA等级 是否允许数据上报: ture 表明儿童, false 表明不是儿童
        ///</summary>
        public void SetCOPPAIsAgeRestrictedUser(bool isChild)
        {
            TPAds.Instance().SetCOPPAIsAgeRestrictedUser(isChild);
        }

        ///<summary>
        ///获取当前 COPPA等级： 0 表明儿童 , 1 表明不是儿童, 2 未设置
        ///</summary>
        public int GetCOPPAIsAgeRestrictedUser()
        {
            return TPAds.Instance().GetCOPPAIsAgeRestrictedUser();
        }

        ///<summary>
        ///设置是否开启个性化推荐广告。 false 关闭 ，true 开启。SDK默认 true 开启
        ///</summary>
        public void SetOpenPersonalizedAd(bool open)
        {
            TPAds.Instance().SetOpenPersonalizedAd(open);
        }

        ///<summary>
        ///当前的个性化状态  false 关闭 ，true 开启
        ///</summary>
        public bool IsOpenPersonalizedAd()
        {
            return TPAds.Instance().IsOpenPersonalizedAd();
        }

        ///<summary>
        ///开启获取AuthId  false 关闭 ，true 开启
        ///</summary>
        public void SetAuthUID(bool needUid)
        {
#if UNITY_ANDROID
                    TPAds.Instance().SetAuthUID(needUid);
#endif
        }

        ///<summary>
        ///清理指定广告位下的广告缓存，一般使用场景：用于切换用户后清除激励视频的缓存广告
        ///</summary>
        public void ClearCache(string adUnitId)
        {
            TPAds.Instance().ClearCache(adUnitId);
        }

        ///<summary>
        ///设置是否关闭广告过期检测，bool autoCheck,默认为true 开启检测
        ///</summary>
        public void SetAutoExpiration(bool autoCheck)
        {
            TPAds.Instance().SetAutoExpiration(autoCheck);
        }

        ///<summary>
        ///主动触发广告检测
        ///</summary>
        public void CheckAutoExpiration()
        {
            TPAds.Instance().CheckAutoExpiration();
        }

        ///<summary>
        ///选择是否只使用TradPlus国内服务器还是全球服务器，bool onlyCn, 默认false 使用全球服务器
        ///</summary>
        public void SetCnServer(bool onlyCn)
        {
            TPAds.Instance().SetCnServer(onlyCn);
        }
        ///<summary>
        ///打开测试工具 集成方法参考：
        ///iOS https://docs.tradplusad.com/docs/integration_ios/sdk_test_android/test_tool/
        ///android https://docs.tradplusad.com/docs/tradplussdk_android_doc_v6/sdk_test_android/test_tool
        ///</summary>
        public void openTradPlusTool(string appId)
        {
            TPAds.Instance().OpenTradPlusTool(appId);
        }

/// 仅android支持的API

///<summary>
///选择是否开启自动加载重新load广告时，是否延迟2秒，bool isopen, 默认false 不使用延迟2s
///仅android支持
///</summary>
        public void SetOpenDelayLoadAds(bool isOpen)
        {
#if UNITY_ANDROID
            TPAds.Instance().SetOpenDelayLoadAds(isOpen);
#endif
        }

        ///<summary>
        ///android 国内隐私权限 是否打开，仅android支持（iOS时返回false）
        ///</summary>
        public bool IsPrivacyUserAgree()
        {
#if UNITY_ANDROID
        return TPAds.Instance().IsPrivacyUserAgree();

#else
        return false;
#endif
        }

        ///<summary>
        ///android 设置国内隐私权限，仅android支持
        ///</summary>
        public void SetPrivacyUserAgree(bool open)
        {
#if UNITY_ANDROID
            TPAds.Instance().SetPrivacyUserAgree(open);
#endif
        }

        ///<summary>
        ///android 设置可使用数据库容量大小，到达数值后自动清空数据库，默认20MB，仅android支持
        ///</summary>
        public void SetMaxDatabaseSize(int size)
        {
#if UNITY_ANDROID
            TPAds.Instance().SetMaxDatabaseSize(size);
#endif
        }

        public void AddGlobalAdImpression(Action<Dictionary<string, object>> OnGlobalAdImpression)
        {
            if(!this.didAddGlobalAdImpressionListener)
            {
                this.didAddGlobalAdImpressionListener = true;
                TPAds.Instance().AddGlobalAdImpressionListener();

            }
            if(this.onGlobalAdImpression != null)
            {
                TPAds.Instance().OnGlobalAdImpression -= this.onGlobalAdImpression;
                this.onGlobalAdImpression = null;
            }
            if(OnGlobalAdImpression != null)
            {
                this.onGlobalAdImpression = OnGlobalAdImpression;
                TPAds.Instance().OnGlobalAdImpression += OnGlobalAdImpression;
            }
        }

        ///<summary>
        ///开发者在 OnApplicationQuit 生命周期时调用关闭回调
        ///仅iOS支持
        ///</summary>
        public void ClearCallback()
        {
#if UNITY_EDITOR

#elif UNITY_IOS
            TPAds.Instance().ClearCallback();
#endif
        }

        ///<summary>
        ///全局的展示回调
        ///</summary>
        public event Action<Dictionary<string, object>> OnGlobalAdImpression;

        ///<summary>
        ///ios UID2StartFinish errorMsg 成功为 null
        ///Android errorMsg 成功为 Success
        ///</summary>
        public event Action<string> OnUID2StartFinish;

        ///<summary>
        ///初始化完成 bool success
        ///</summary>
        public event Action<bool> OnInitFinish;

        ///<summary>
        ///查询地区成功 bool isEu 是否欧洲, bool isCn 是否中国, bool isCa 是否加州
        ///</summary>
        public event Action<bool,bool,bool> OnCurrentAreaSuccess;

        ///<summary>
        ///查询地区失败，开发者需要自行查询或处理，设置各隐私权限。
        ///</summary>
        public event Action<string> OnCurrentAreaFailed;


        public TradplusAds()
        {
            TPAds.Instance().OnInitFinish += (success) =>
            {
                if (this.OnInitFinish != null)
                {
                    this.OnInitFinish(success);
                }
            };

            TPAds.Instance().OnCurrentAreaSuccess += (isEu,isCn,isCa) =>
            {
                if (this.OnCurrentAreaSuccess != null)
                {
                    this.OnCurrentAreaSuccess(isEu, isCn, isCa);
                }
            };

            TPAds.Instance().OnCurrentAreaFailed += (msg) =>
            {
                if (this.OnCurrentAreaFailed != null)
                {
                    this.OnCurrentAreaFailed(msg);
                }
            };

            TPAds.Instance().OnUID2StartFinish += (errorMsg) =>
            {
                if (this.OnUID2StartFinish != null)
                {
                    this.OnUID2StartFinish(errorMsg);
                }
            };
        }
    }
}