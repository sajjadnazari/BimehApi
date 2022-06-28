using System.Collections.Generic;
using System.Linq;

namespace Common.Bimeh.DTos.Common
{
    public class ServiceResult : ServiceResult<string>
    {
        public new ServiceResult AddResult<TParameter>(ServiceResult<TParameter> serviceResult, bool isAddSuccessMessage = false)
          => (ServiceResult)base.AddResult(serviceResult, false);

        public new ServiceResult AddResult(string messageText, bool hasError)
            => (ServiceResult)AddResult(messageText, hasError, default);
    }
    public class ServiceResult<T>
    {
        public List<ServiceResult<T>> MessageItems { get; } = new List<ServiceResult<T>>();
        public ServiceResult()
        {

        }

        public ServiceResult(string messageText) => AddResult(messageText, HasError, default);
        public ServiceResult(string messageText, bool hasError) => AddResult(messageText, hasError, default);
        public ServiceResult(string messageText, bool hasError, T data) => AddResult(messageText, hasError, data);

        public ServiceResult<T> AddResult(string messageText, bool hasError = false) => AddResult(messageText, hasError, default(T));

        public ServiceResult<T> AddResult(string messageText, bool hasError, T data)
        {
            this.Data = data;
            this.HasError = hasError;
            this.Message = messageText;
            return this;
        }

        public ServiceResult<T> AddResult<TParameter>(ServiceResult<TParameter> serviceResult, bool isAddSuccessMessage = false)
        {
            serviceResult.MessageItems
                ?.Where(el => isAddSuccessMessage || el.HasError)
                .ToList()
                .ForEach(el => AddResult(el.Message, el.HasError, typeof(T) == typeof(TParameter) ? (dynamic)el.Data : default(T)));

            return this;
        }

        public bool HasError { get; set; } = false;

        public string Message { get; set; }

        public T Data { get; set; }

    }
}
