using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Collections;

namespace Laugris.Sage
{
    public class StaticPropertyHelper : ICustomTypeDescriptor
    {
        private Type classType;

        public StaticPropertyHelper(Type classType)
        {
            this.classType = classType;
        }

        public System.ComponentModel.TypeConverter GetConverter()
        {
            return null;
        }

        public System.ComponentModel.EventDescriptorCollection GetEvents(System.Attribute[] attributes)
        {
            return null;
        }

        public System.ComponentModel.EventDescriptorCollection GetEvents()
        {
            return null;
        }

        public string GetComponentName()
        {
            return null;
        }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd)
        {
            return this.classType;
        }

        public System.ComponentModel.AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this.classType);
        }

        public PropertyDescriptorCollection GetProperties(System.Attribute[] attributes)
        {
            PropertyInfo[] propInfos =
            this.classType.GetProperties(BindingFlags.Public | BindingFlags.Static);

            ArrayList props = new ArrayList(propInfos.Length);

            foreach (PropertyInfo propInfo in propInfos)
            {
                props.Add(new ReflectPropertyDescriptor(propInfo, 
                    Attribute.GetCustomAttributes(propInfo)));
            }

            return new PropertyDescriptorCollection(
            (PropertyDescriptor[])props.ToArray(typeof(PropertyDescriptor)));
        }

        public System.ComponentModel.PropertyDescriptorCollection GetProperties()
        {
            return null;
        }

        public object GetEditor(System.Type editorBaseType)
        {
            return null;
        }

        public System.ComponentModel.PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        public System.ComponentModel.EventDescriptor GetDefaultEvent()
        {
            return null;
        }

        public string GetClassName()
        {
            return this.classType.Name;
        }
    }

    public class ReflectPropertyDescriptor : PropertyDescriptor
    {
        private PropertyInfo propInfo;

        public ReflectPropertyDescriptor(PropertyInfo propInfo) :
            base(propInfo.Name, null)
        {
            this.propInfo = propInfo;
        }

        public ReflectPropertyDescriptor(PropertyInfo propInfo, Attribute[] attrs) :
            base(propInfo.Name, attrs)
        {
            this.propInfo = propInfo;
        }

        public override void SetValue(object component, object value)
        {
            this.propInfo.DeclaringType.InvokeMember(
            this.propInfo.Name,
            BindingFlags.Static | BindingFlags.Public | BindingFlags.SetProperty,
            null,
            null,
            new object[] { value });
        }

        public override object GetValue(object component)
        {
            return this.propInfo.DeclaringType.InvokeMember(
            this.propInfo.Name,
            BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty,
            null,
            null,
            new object[] { });
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override System.Type ComponentType
        {
            get
            {
                return this.propInfo.ReflectedType;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return !this.propInfo.CanWrite;
            }
        }

        public override System.Type PropertyType
        {
            get
            {
                return this.propInfo.PropertyType;
            }
        }

        public override void ResetValue(object component)
        {

        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }
    }
}
