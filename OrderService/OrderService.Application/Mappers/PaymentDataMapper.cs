namespace OrderService.Application.Mappers;

public static class PaymentDataMapper
{
    public static Domain.Entities.PaymentData MapToEntity(this Models.PaymentData paymentData)
    {
        return new Domain.Entities.PaymentData
        {
            Id = paymentData.Id,
            PaymentMethod = paymentData.PaymentMethod,
            CardNumber = paymentData.CardNumber,
        };
    }

    public static List<Domain.Entities.PaymentData> MapToEntity(this List<Models.PaymentData> paymentDatas)
    {
        return paymentDatas.Select(pd => pd.MapToEntity()).ToList();
    }

    public static Models.PaymentData MapToModel(this Domain.Entities.PaymentData paymentData)
    {
        return new Models.PaymentData
        {
            Id = paymentData.Id,
            PaymentMethod = paymentData.PaymentMethod,
            CardNumber = paymentData.CardNumber,
        };
    }

    public static List<Models.PaymentData> MapToModel(this List<Domain.Entities.PaymentData> paymentDatas)
    {
        return paymentDatas.Select(pd => pd.MapToModel()).ToList();
    }
}
