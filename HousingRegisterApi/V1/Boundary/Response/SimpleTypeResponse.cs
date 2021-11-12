namespace HousingRegisterApi.V1.Boundary.Response
{
    /// <summary>
    /// Simple type response for the front end like strings and numbers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleTypeResponse<T>
    {
        public T Value { get; }

        public SimpleTypeResponse(T value)
        {
            Value = value;
        }
    }
}
