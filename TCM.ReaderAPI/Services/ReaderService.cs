using System.Reflection;

namespace TCM.ReaderAPI.Services
{
    public class ReaderService: IHostedService
    {
        private Type? objType;
        private object? obj;
        private const int NOT_LOADED_ERROR_CODE = -1;
        private const string NOT_LOADED_ERROR_CODE_DESCRIPTION = "Ошибка загрузки компоненты!";

        public string ResultCodeDescription
        {
            get
            {
                var value = getProperty("ResultCodeDescription") ?? NOT_LOADED_ERROR_CODE_DESCRIPTION;
                return (string)value;
            }
        }
        public int ResultCode
        {
            get
            {
                var value = getProperty("ResultCode") ?? NOT_LOADED_ERROR_CODE;
                return (int)value;
            }
        }
        public string SmartCardNativeId
        {
            get
            {
                var value = getProperty("SmartCardNativeId") ?? "";
                return (string)value;
            }
        }


        private object? getProperty(string name)
        {
            object? value = null;
            if (objType != null)
            {
                value = objType.InvokeMember(name, BindingFlags.GetProperty, null, obj, null);
            }

            return value;
        }
        private void setProperty(string name, object value)
        {
            if (objType != null)
            {
                objType.InvokeMember(name, BindingFlags.SetProperty, null, obj, new object[] { value });
            }
        }
        private bool executeMethod(string methodName)
        {
            return (int)objType.InvokeMember(methodName, BindingFlags.InvokeMethod, null, obj, new object[] { }) == 0;
        }
        public bool Init()
        {
            bool result = true;
            if (objType == null)
            {
                objType = Type.GetTypeFromProgID("OpenAgami.OnlineCardOperationsDriver");
                if (objType != null)
                {
                    try
                    {
                        obj = Activator.CreateInstance(objType);
                    }
                    catch (Exception ex) 
                    {
                        result = false;
                    }
                }
            }

            return result;
        }
        public bool ReadCard()
        {
            bool result = false;
            if(Init())
            {
                result = executeMethod("ReadCard");
            }

            return result;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Init();

            if (ResultCode == 0)
            {
                return Task.CompletedTask;
            }
            else
            {
                return Task.FromException(new Exception(ResultCodeDescription));
            }
        }
        public Task StopAsync(CancellationToken cancellationToken) 
        {
            obj = null;
            objType = null;

            return Task.CompletedTask;
        }
    }
}
