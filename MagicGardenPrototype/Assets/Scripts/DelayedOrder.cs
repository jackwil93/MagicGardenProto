using System.Collections.Generic;

[System.Serializable]
public class DelayedOrder {
    public int minutesDelayed;
    public List<Order> ordersInPack = new List<Order>();
}

public class Order
{
    public enum orderType { email, item};
    public orderType myOrderType;
    public string orderID;
    public int orderAmount;
}



