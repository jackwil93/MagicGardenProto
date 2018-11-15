using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.SimpleAndroidNotifications;

public class DelayedOrderManager : MonoBehaviour
{

    GameManager GM;
    EmailManager EM;
    public List<DelayedOrder> delayedOrders = new List<DelayedOrder>();

    private void Start()
    {
        GM = GetComponent<GameManager>();
        EM = GetComponent<EmailManager>();

        InvokeRepeating("CheckOrders", 0, 60);
    }

    public void AddNewOrder(Order newOrder, int delayTimeMins, string notificationMessage)
    {
        Debug.Log("New order..." + newOrder.orderID);
        DelayedOrder newDelayOrder = new DelayedOrder();

        Debug.Log("Created newDelayedOrder");
        newDelayOrder.ordersInPack.Add(newOrder);
        newDelayOrder.minutesDelayed = delayTimeMins;

        delayedOrders.Add(newDelayOrder);

        // Set Up Phone Notification
        NotificationManager.SendWithAppIcon(System.TimeSpan.FromMinutes(delayTimeMins), "Magic Proto", notificationMessage, Color.white, NotificationIcon.Bell);

        Debug.Log("New DelayedOrder Added. An " + newDelayOrder.ordersInPack[0].myOrderType + " order to arrive in " + newDelayOrder.minutesDelayed + " mins");
    }

    public List<DelayedOrder> GetAllDelayedOrders() // Called by XMLSaveLoad to put to PlayerData
    {
        return delayedOrders;
    }

    public void AddListOfDelayedOrders(List<DelayedOrder> newList, int minutesToSubtract) // Called by GameManager to take orders from PlayerData to stored List
    {
        delayedOrders.AddRange(newList);
        CheckOrders(minutesToSubtract);
    }

    void CheckOrders()
    {
        CheckOrders(0);
    }

    public void CheckOrders(int minutesPassed) // Parameter here is called from GameManager after calculating time passed since last play
    {
        Debug.Log("Checking " + delayedOrders.Count + " Orders at: " + System.DateTime.Now.ToString("yyyy,MM,dd,HH:mm.ss,FFF"));

        if (minutesPassed == 0) // If Called from standard CheckOrders(), this only happens once a minute via Invoke
            minutesPassed = 1;


        if (delayedOrders.Count > 0)
        {
            foreach (DelayedOrder order in delayedOrders)
            {
                order.minutesDelayed -= minutesPassed;
                if (order.minutesDelayed <= 0)
                    DeliverOrder(order);
            }

            delayedOrders.RemoveAll(Delivered);
        }


        Debug.Log("Finished Checking Orders at: " + System.DateTime.Now.ToString("yyyy,MM,dd,HH:mm.ss,FFF"));

    }

    bool Delivered(DelayedOrder order)
    {
        if (order.minutesDelayed <= 0)
            return true;
        else
            return false;
    }

    void DeliverOrder(DelayedOrder readyOrder)
    {
        foreach (Order order in readyOrder.ordersInPack)
        {
            if (order.myOrderType == Order.orderType.email)
            {
                Debug.Log("Delivering an Email" + " | contentID = " + readyOrder.ordersInPack[0].orderID);
                EM.PutNextEmailToConversation(order.orderID);
            }
            else if (order.myOrderType == Order.orderType.item)
            {
                Debug.Log("Delivering an Item| " + order.orderAmount + "x " + order.orderID);
            }

        }
    }
}
