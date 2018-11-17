using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
[XmlType("DelayedOrder")]
public class DelayedOrder {
    public int minutesDelayed;
    public List<Order> ordersInPack = new List<Order>();
}

[System.Serializable]
public class Order
{
    public enum orderType { email, item};
    public orderType myOrderType;
    public string orderID;
    public int orderAmount;
}



