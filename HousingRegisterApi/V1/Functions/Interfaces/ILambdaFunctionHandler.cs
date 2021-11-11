using Amazon.Lambda.Core;

namespace HousingRegisterApi.V1.Functions
{
    public interface ILambdaFunctionHandler
    {
        void Handle(ILambdaContext context);
    }

    public interface ILambdaFunctionHandler<TOut>
    {
        TOut Handle(ILambdaContext context);
    }

    public interface ILambdaFunctionHandler<TIn, TOut>
    {
        TOut Handle(TIn input, ILambdaContext context);
    }
}
