using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AnyThinkAds.Api;
using AnyThinkAds.ThirdParty.LitJson;

namespace Watermelon
{
    public class ATContainer : ATSDKInitListener
    {
#if UNITY_ANDROID
        private const string SdkAppId = "h6943be368bb6e";
        private const string SdkKey = "a9a4b725c5d9cdc6817c5f42d9e1c0c98";
#elif UNITY_IOS || UNITY_IPHONE
        private const string SdkAppId = "a5b0e8491845b3";
        private const string SdkKey = "7eae0567827cfe2b22874061763f30c9";
#endif
        public ATContainer()
        {

        }

        public void initSDK()
        {
            //（可选配置）设置自定义的Map信息，可匹配后台配置的广告商顺序的列表（App纬度）
            //注意：调用此方法会清除setChannel()、setSubChannel()方法设置的信息，如果有设置这些信息，请在调用此方法后重新设置
            //ATSDKAPI.initCustomMap(new Dictionary<string, string> { { "unity3d_data", "test_data" } });

            ////（可选配置）设置自定义的Map信息，可匹配后台配置的广告商顺序的列表（Placement纬度）
            //// ATSDKAPI.setCustomDataForPlacementID(new Dictionary<string, string> { { "unity3d_data_pl", "test_data_pl" } }, placementId);

            ////（可选配置）设置渠道的信息，开发者可以通过该渠道信息在后台来区分看各个渠道的广告数据
            //// 注意：如果有使用initCustomMap()方法，必须在initCustomMap()方法之后调用此方法
            //ATSDKAPI.setChannel("unity3d_test_channel");

            ////（可选配置）设置子渠道的信息，开发者可以通过该渠道信息在后台来区分看各个渠道的子渠道广告数据
            ////注意：如果有使用initCustomMap()方法，必须在initCustomMap()方法之后调用此方法
            //ATSDKAPI.setSubChannel("unity3d_test_subchannel");

            ////设置开启Debug日志（强烈建议测试阶段开启，方便排查问题）
            //ATSDKAPI.setLogDebug(true);

            ////app - id：h6943be368bb6e
            ////app - key： a9a4b725c5d9cdc6817c5f42d9e1c0c98
            ////插屏：n66a0bb5925c27
            ////激励：n66ebda2670e48
            ////（必须配置）SDK的初始化
            //ATSDKAPI.initSDK(SdkAppId, SdkKey);//Use your own app_id & app_key here
            //Debug.Log("[A] ATContainer initSDK");
        }

        public void initSuccess()
        {
            Debug.Log("[A] ATContainer initSuccess");
        }

        public void initFail(string message)
        {
            Debug.Log("[A] ATContainer initFail: " + message);
        }
    }
}
