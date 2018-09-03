using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[CreateAssetMenu]
[XmlRoot("CharacterEmailLog")]
public class EmailCharacter: ScriptableObject{
    [XmlAttribute("characterName")]
    public string characterName;
    [XmlArray("Emails")]
    [XmlArrayItem("Email")]
    public List<EmailResponse> emailList;
}
