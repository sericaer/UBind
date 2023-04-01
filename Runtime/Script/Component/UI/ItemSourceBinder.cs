
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Aya.DataBinding
{
    [AddComponentMenu("Data Binding/ItemSource Binder")]
    public class ItemSourceBinder : ComponentBinder<LayoutGroup, IEnumerable<object>, RuntimeItemSourceBinder>
    {
        public override bool NeedUpdate => true;

        public GameObject ItemTemplate;

        public override RuntimeItemSourceBinder CreateDataBinder()
        {
            var dataBinder = new RuntimeItemSourceBinder
            {
                Target = Target,
                Context = Context,
                Direction = Direction,
                Key = Key,
                ItemTemplate = ItemTemplate
            };

            return dataBinder;
        }

        public override void Awake()
        {
            ItemTemplate.SetActive(false);
        }
    }

    public class RuntimeItemSourceBinder : DataBinder<LayoutGroup, IEnumerable<object>>
    {
        public override bool NeedUpdate => true;

        public GameObject ItemTemplate { get; set; }

        private Dictionary<object, GameObject> itemData2Obj = new Dictionary<object, GameObject>();

        public override IEnumerable<object> Value
        {
            get => _items;
            set
            {
                foreach(var gmObj in itemData2Obj.Values)
                {
                    Object.Destroy(gmObj);
                }

                foreach (var key in itemData2Obj.Keys)
                {
                    itemData2Obj.Remove(key);
                }

                _items = value;
            }
        }



        public override void UpdateSource()
        {
            base.UpdateSource();

            if(_items == null)
            {
                return;
            }

            foreach (var item in _items)
            {
                if(!itemData2Obj.ContainsKey(item))
                {
                    var instance = Object.Instantiate(ItemTemplate, ItemTemplate.transform.parent) as GameObject;
                    itemData2Obj.Add(item, instance);

                    var contextString = instance.GetInstanceID().ToString();
                    UpdateUIComponentContextString(contextString, instance);

                    instance.SetActive(true);

                    BindMap.Bind(item, contextString);
                }
            }

            for(int i=0; i<itemData2Obj.Count; i++)
            {
                var pair = itemData2Obj.ElementAt(i);

                if (!_items.Contains(pair.Key))
                {
                    Object.Destroy(itemData2Obj[pair.Key]);
                    itemData2Obj.Remove(pair.Key);
                    BindMap.UnBind(pair.Key);
                }

            }  
        }

        private IEnumerable<object> _items;


        private static void UpdateUIComponentContextString(string contextStr, GameObject gameObject)
        {
            var componentBinders = gameObject.GetComponentsInChildren<ComponentBinderBase>();
            foreach (var binder in componentBinders)
            {
                binder.Context = gameObject.GetInstanceID().ToString();
                Debug.Log("Change binder.Context");
            }
        }

        private static void UpdateItemContextString(string contextStr, object item)
        {
            var bindMap = BindMap.GetBindMap(item);
            foreach (var kv in bindMap.PropertyInfos)
            {
                kv.Value.Context = contextStr;

                Debug.Log($"kv.Value.Context = {kv.Value.Context}");
            }

            foreach (var kv in bindMap.FieldInfos)
            {
                kv.Value.Context = contextStr;

                Debug.Log($"kv.Value.Context = {kv.Value.Context}");
            }
        }
    }

#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(ItemSourceBinder)), UnityEditor.CanEditMultipleObjects]
    public class ItemSourceBinderEditor : ComponentBinderEditor<LayoutGroup, IEnumerable<object>, RuntimeItemSourceBinder>
    {
        public ItemSourceBinder Binder => target as ItemSourceBinder;
        protected SerializedProperty ItemTemplate;

        public override void OnEnable()
        {
            base.OnEnable();
            ItemTemplate = serializedObject.FindProperty(nameof(Binder.ItemTemplate));
        }

        public override void DrawBody()
        {
            EditorGUILayout.PropertyField(ItemTemplate);
        }
    }

#endif
}
