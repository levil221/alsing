namespace QI4N.Framework.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public abstract class AbstractPropertiesModel
    {
        private readonly IList<PropertyModel> propertyModels = new List<PropertyModel>();

        public void AddPropertyFor(PropertyInfo propertyInfo, Type compositeType)
        {
            PropertyModel propertyModel = NewPropertyModel(propertyInfo, compositeType);
            this.propertyModels.Add(propertyModel);
        }

        public StateHolder NewBuilderInstance()
        {
            var properties = new Dictionary<PropertyInfo, AbstractProperty>();
            foreach (PropertyModel propertyModel in this.propertyModels)
            {
                AbstractProperty property = propertyModel.NewBuilderInstance();
                properties.Add(propertyModel.PropertyInfo, property);
            }

            return new PropertiesInstance(properties);
        }

        public StateHolder NewInitialInstance()
        {
            var properties = new Dictionary<PropertyInfo, AbstractProperty>();
            foreach (PropertyModel propertyModel in this.propertyModels)
            {
                AbstractProperty property = propertyModel.NewInitialInstance();
                properties.Add(propertyModel.PropertyInfo, property);
            }

            return new PropertiesInstance(properties);
        }

        public StateHolder NewInstance(StateHolder state)
        {
            var properties = new Dictionary<PropertyInfo, AbstractProperty>();
            foreach (PropertyModel propertyModel in this.propertyModels)
            {
                object initialValue = state.GetProperty(propertyModel.GetMethod).Value;

                initialValue = CloneInitialValue(initialValue, false);

                // Create property instance
                AbstractProperty property = propertyModel.NewInstance(initialValue);
                properties.Add(propertyModel.PropertyInfo, property);
            }
            return new PropertiesInstance(properties);
        }


        protected static PropertyModel NewPropertyModel(PropertyInfo propertyInfo, Type compositeType)
        {
            PropertyModel model = new PropertyModelImpl(propertyInfo);

            return model;
        }

        private static object CloneInitialValue(object initialValue, bool immutable)
        {
            if (initialValue is ICloneable)
            {
                var c = initialValue as ICloneable;
                return c.Clone();
            }

            return initialValue;
        }
    }
}