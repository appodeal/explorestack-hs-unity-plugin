using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AOT;
using StackHolisticSolution.Api;
using StackHolisticSolution.Common;
using UnityEngine;

namespace StackHolisticSolution.Platforms.iOS
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public class iOSHSLogger : IHSLogger
    {
        private readonly HSLoggerObjCBridge hsLoggerObjCBridge;

        public iOSHSLogger()
        {
            hsLoggerObjCBridge = new HSLoggerObjCBridge();
        }
        
        public void setEnabled(bool value)
        {
            hsLoggerObjCBridge.setEnabled(true);
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class iOSHSAppodealConnector : IHSAppodealConnector
    {
        private readonly HSAppodealConnectorObjCBridge hsAppodealConnectorObjCBridge;

        public iOSHSAppodealConnector()
        {
            hsAppodealConnectorObjCBridge = new HSAppodealConnectorObjCBridge();
        }
        
        public IntPtr getIntPtr()
        {
            return hsAppodealConnectorObjCBridge.getIntPtr();
        }

        public void setEventsEnabled(bool value)
        {
            hsAppodealConnectorObjCBridge.setEventsEnabled(value);
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class iOSHSAppsflyerService : IHSAppsflyerService
    {
        private readonly HSAppsflyerServiceObjCBridge hsAppsflyerServiceObjCBridge;

        public iOSHSAppsflyerService(string key)
        {
            hsAppsflyerServiceObjCBridge = new HSAppsflyerServiceObjCBridge(key);
        }
        
        public IntPtr GetIntPtr()
        {
            return hsAppsflyerServiceObjCBridge.getIntPtr();
        }
        
        public void setEventsEnabled(bool value)
        {
            hsAppsflyerServiceObjCBridge.setEventsEnabled(value);
        }

        public AndroidJavaObject GetAndroidInstance()
        {
            Debug.Log("Not supported");
            return null;
        }
    }
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class iOSHSFirebaseService : IHSFirebaseService
    {
        private readonly HSFirebaseServiceObjCBridge hSFirebaseServiceObjCBridge;

        public iOSHSFirebaseService()
        {
            hSFirebaseServiceObjCBridge = new HSFirebaseServiceObjCBridge();
        }
        
        public IntPtr GetIntPtr()
        {
            return hSFirebaseServiceObjCBridge.getIntPtr();
        }
        
        public void setEventsEnabled(bool value)
        {
            hSFirebaseServiceObjCBridge.setEventsEnabled(value);
        }

        public AndroidJavaObject GetAndroidInstance()
        {
            Debug.Log("Not supported");
            return null;
        }
    }
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class iOSHSFacebookService : IHSFacebookService
    {
        private readonly HSFacebookServiceObjCBridge hSFacebookServiceObjCBridge;

        public iOSHSFacebookService()
        {
            hSFacebookServiceObjCBridge = new HSFacebookServiceObjCBridge();
        }

        public IntPtr GetIntPtr()
        {
            return hSFacebookServiceObjCBridge.getIntPtr();
        }
        
        public void setEventsEnabled(bool value)
        {
            hSFacebookServiceObjCBridge.setEventsEnabled(value);
        }

        public AndroidJavaObject GetAndroidInstance()
        {
            Debug.Log("Not supported");
            return null;
        }

        
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class iOSHSAppConfig : IHSAppConfig
    {
        private readonly HSAppConfigObjCBridge hSAppConfigObjCBridge;
        
        public iOSHSAppConfig()
        {
            hSAppConfigObjCBridge = new HSAppConfigObjCBridge();
        }

        public IntPtr getIntPtr()
        {
           return hSAppConfigObjCBridge.getIntPtr();
        }
        
        public void withConnectors(HSAppodealConnector hsAppodealConnector)
        {
            var iOSHSAppodealConnector = (iOSHSAppodealConnector) hsAppodealConnector.getHSAppodealConnector();
            hSAppConfigObjCBridge.withConnectors(iOSHSAppodealConnector.getIntPtr());
        }

        public void withServices(params IHSService[] services)
        {
            hSAppConfigObjCBridge.withServices(services.Select
                (service => service.GetIntPtr()).ToArray());
        }

        public void setDebugEnabled(bool value)
        {
            hSAppConfigObjCBridge.setDebugEnabled(value);
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class iOSHSApp : IHSApp
    {
        private readonly HSAppObjCBridge hsAppObjCBridge;
        private static IHSAppInitializeListener hsAppInitializeListener;
        private static IHSInAppPurchaseValidateListener hsInAppPurchaseValidateListener;

        public iOSHSApp()
        {
            hsAppObjCBridge = new HSAppObjCBridge();
        }

        public void initialize(HSAppConfig appConfig, IHSAppInitializeListener listener)
        {
            hsAppInitializeListener = listener;
            var iOSAppConfig = (iOSHSAppConfig) appConfig.getHSAppConfig();
            hsAppObjCBridge.initialize(iOSAppConfig.getIntPtr(), onAppInitialized);
        }

        public void logEvent(string key, Dictionary<string, object> dictionary)
        {
            hsAppObjCBridge.logEvent(key, dictionary);
        }

        public void logEvent(string key)
        {
            hsAppObjCBridge.logEvent(key);
        }

        public void validateInAppPurchase(HSInAppPurchase purchase, IHSInAppPurchaseValidateListener listener)
        {
            hsInAppPurchaseValidateListener = listener;
            var inAppPurchase = (iOSHSInAppPurchase) purchase.getNativeHSInAppPurchase();
            hsAppObjCBridge.validateInAppPurchase(inAppPurchase.getIntPtr(), onInAppPurchaseValidateSuccess, onInAppPurchaseValidateFail);
        }
        
        #region HSAppInitializeListener delegate

        [MonoPInvokeCallback(typeof(HSAppInitializeListener))]
        private static void onAppInitialized(IntPtr error)
        {
            var hsErrors = new List<HSError> { new HSError(new iOSHSError(error))};
            hsAppInitializeListener?.onAppInitialized(hsErrors);
        }
        
        #endregion
        
        #region InAppPurchaseValidateListener delegate

        [MonoPInvokeCallback(typeof(InAppPurchaseValidateSuccess))]
        private static void onInAppPurchaseValidateSuccess(IntPtr purchase,IntPtr error)
        {
            var hsErrors = new List<HSError> { new HSError(new iOSHSError(error))};
            hsInAppPurchaseValidateListener?.onInAppPurchaseValidateSuccess(
                new HSInAppPurchase(new iOSHSInAppPurchase(purchase)),hsErrors );
        }
        
        [MonoPInvokeCallback(typeof(InAppPurchaseValidateFail))]
        private static void onInAppPurchaseValidateFail(IntPtr error)
        {
            var hsErrors = new List<HSError> { new HSError(new iOSHSError(error))};
            hsInAppPurchaseValidateListener?.onInAppPurchaseValidateFail(hsErrors);
        }
        
        #endregion
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class iOSHSError : IHSError
    {
        private readonly HSErrorObjCBridge hSErrorObjCBridge;

        public iOSHSError(IntPtr error)
        {
            hSErrorObjCBridge = new HSErrorObjCBridge(error);
        }

        public IntPtr getIntPtr()
        {
            return hSErrorObjCBridge.getIntPtr();
        }

        public string toString()
        {
            return hSErrorObjCBridge.toString();
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class iOSHSInAppPurchase : IHSInAppPurchase
    {
        private readonly HSInAppPurchaseObjCBridge hsInAppPurchaseObjBridge;

        public iOSHSInAppPurchase(IntPtr builder)
        {
            hsInAppPurchaseObjBridge = new HSInAppPurchaseObjCBridge(builder);
        }

        public IntPtr getIntPtr()
        {
            return hsInAppPurchaseObjBridge.getIntPtr();
        }

        public string getPublicKey()
        {
            return hsInAppPurchaseObjBridge.getPublicKey();
        }

        public string getSignature()
        {
            return hsInAppPurchaseObjBridge.getSignature();
        }

        public string getPurchaseData()
        {
            return hsInAppPurchaseObjBridge.getPurchaseData();
        }

        public string getPrice()
        {
            return hsInAppPurchaseObjBridge.getPrice();
        }

        public string getCurrency()
        {
            return hsInAppPurchaseObjBridge.getCurrency();
        }

        public string getAdditionalParameters()
        {
            return hsInAppPurchaseObjBridge.getAdditionalParameters();
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class iOSHSInAppPurchaseBuilder : IHSInAppPurchaseBuilder
    {
        private readonly HSInAppPurchaseBuilderObjCBridge hsInAppPurchaseBuilderObjCBridge;

        public iOSHSInAppPurchaseBuilder()
        {
            hsInAppPurchaseBuilderObjCBridge = new HSInAppPurchaseBuilderObjCBridge();
        }

        private IntPtr GetIntPtr()
        {
            return hsInAppPurchaseBuilderObjCBridge.getIntPtr();
        }

        public void withAdditionalParams(Dictionary<string, string> additionalParameters)
        {
            hsInAppPurchaseBuilderObjCBridge.withAdditionalParams(additionalParameters);
        }

        public void withCurrency(string currency)
        {
            hsInAppPurchaseBuilderObjCBridge.withCurrency(currency);
        }

        public void withPrice(string price)
        {
            hsInAppPurchaseBuilderObjCBridge.withCurrency(price);
        }

        public void withPurchaseData(string purchaseData)
        {
            hsInAppPurchaseBuilderObjCBridge.withPurchaseData(purchaseData);
        }

        public void withSignature(string signature)
        {
            hsInAppPurchaseBuilderObjCBridge.withSignature(signature);
        }

        public void withPublicKey(string publicKey)
        {
            hsInAppPurchaseBuilderObjCBridge.withPublicKey(publicKey);
        }

        IHSInAppPurchase IHSInAppPurchaseBuilder.build()
        {
            return new iOSHSInAppPurchase(GetIntPtr());
        }
    }
}