using System.Reflection;

namespace TCM.ReaderAPI.Services
{
    public class ReaderService: IHostedService
    {
        private Type? objType;
        private object? obj;
        private string? resultDescription;
        private int resultCode;

        public string ResultDescription
        {
            get { return resultDescription ?? ""; }
        }
        public int ResultCode
        {
            get { return resultCode; }
        }

        private void setError(string error, int code = 1)
        {
            resultDescription = error;
            resultCode = code;
        }
        private void resetError()
        {
            resultDescription = "Ошибок нет.";
            resultCode = 0;
        }


        public bool Init()
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
                    setError(ex.Message);
                }
            }
            else
            {
                setError("Ошибка загрузки компоненты. Возможно компонента не была установлена!");
            }

            return resultCode == 0;
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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Init();

            if (resultCode == 0)
            {
                return Task.CompletedTask;
            }
            else
            {
                return Task.FromException(new Exception(resultDescription));
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
