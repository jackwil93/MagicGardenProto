using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[CreateAssetMenu]
public class EmailResponse : ScriptableObject{
    [XmlAttribute("emailID")]
    public string emailID;
    public string content;
    public string itemOrder;
    public string playerReplyNeutral;
    public string playerReplyGood;
    public string playerReplyBad;
    public string nextEmailIDNeutral;
    public string nextEmailIDGood;
    public string nextEmailIDBad;
}
