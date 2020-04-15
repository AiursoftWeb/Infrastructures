namespace Aiursoft.Handler.Abstract.Models
{
    public class AiurValue<T> : AiurProtocol
    {
        public AiurValue(T value)
        {
            Value = value;
        }
        public T Value { get; set; }
    }
}
