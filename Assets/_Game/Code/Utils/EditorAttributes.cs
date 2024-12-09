using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Client.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EditorTitle : Attribute
    {
        public string Title;

        public EditorTitle(string title)
        {
            Title = title;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EditorDescription : Attribute
    {
        public string Description;

        public EditorDescription(string description)
        {
            Description = description;
        }
    }
}
