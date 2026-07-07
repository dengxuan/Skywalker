namespace Skywalker.Sample.Website.Application;

public record RegisterMemberInput(string Email, string Phone, string DisplayName);

public record VerifyMemberInput(Guid MemberId, string Code);

public record MemberDto(Guid Id, string Email, string Phone, string DisplayName, bool IsVerified);

public record ProductDto(Guid Id, string Name, decimal Price, int Stock);

public record UpdatePriceInput(decimal Price);

public record PlaceOrderInput(string CustomerEmail, List<PlaceOrderItemInput> Items);

public record PlaceOrderItemInput(Guid ProductId, int Quantity);

public record OrderDto(Guid Id, string OrderNo, string CustomerEmail, decimal TotalAmount, decimal PayableAmount, string Status, List<OrderItemDto> Items);

public record OrderItemDto(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity);
