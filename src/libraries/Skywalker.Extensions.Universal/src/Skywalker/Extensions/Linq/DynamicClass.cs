﻿using System.Dynamic;
using System.Reflection;

namespace Skywalker.Extensions.Linq;

/// <summary>
/// Provides a base class for dynamic objects.
/// 
/// In addition to the methods defined here, the following items are added using reflection:
/// - default constructor
/// - constructor with all the properties as parameters (if not linq-to-entities)
/// - all properties (also with getter and setters)
/// - ToString() method
/// - Equals() method
/// - GetHashCode() method
/// </summary>
public abstract class DynamicClass : DynamicObject
{
    private Dictionary<string, object?>? _propertiesDictionary;

    private Dictionary<string, object?> Properties
    {
        get
        {
            if (_propertiesDictionary == null)
            {
                _propertiesDictionary = new Dictionary<string, object?>();

                foreach (var pi in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var parameters = pi.GetIndexParameters().Length;
                    if (parameters > 0)
                    {
                        // The property is an indexer, skip this.
                        continue;
                    }

                    _propertiesDictionary.Add(pi.Name, pi.GetValue(this, null));
                }
            }

            return _propertiesDictionary;
        }
    }

    /// <summary>
    /// Gets the dynamic property by name.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>T</returns>
    public T? GetDynamicPropertyValue<T>(string propertyName)
    {
        var type = GetType();
        var propInfo = type.GetProperty(propertyName);

        return (T?)propInfo?.GetValue(this, null);
    }

    /// <summary>
    /// Gets the dynamic property value by name.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>value</returns>
    public object? GetDynamicPropertyValue(string propertyName)
    {
        return GetDynamicPropertyValue<object>(propertyName);
    }

    /// <summary>
    /// Sets the dynamic property value by name.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="value">The value.</param>
    public void SetDynamicPropertyValue<T>(string propertyName, T value)
    {
        var type = GetType();
        var propInfo = type.GetProperty(propertyName);

        propInfo?.SetValue(this, value, null);
    }

    /// <summary>
    /// Sets the dynamic property value by name.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="value">The value.</param>
    public void SetDynamicPropertyValue(string propertyName, object value)
    {
        SetDynamicPropertyValue<object>(propertyName, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="object"/> with the specified name.
    /// </summary>
    /// <value>The <see cref="object"/>.</value>
    /// <param name="name">The name.</param>
    /// <returns>Value from the property.</returns>
    public object? this[string name]
    {
        get
        {
            if (Properties.TryGetValue(name, out var result))
            {
                return result;
            }

            return null;
        }

        set
        {
            if (Properties.ContainsKey(name))
            {
                Properties[name] = value;
            }
            else
            {
                Properties.Add(name, value);
            }
        }
    }

    /// <summary>
    /// Returns the enumeration of all dynamic member names.
    /// </summary>
    /// <returns>
    /// A sequence that contains dynamic member names.
    /// </returns>
    public override IEnumerable<string> GetDynamicMemberNames()
    {
        return Properties.Keys;
    }

    /// <summary>
    /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
    /// </summary>
    /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
    /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
    /// <returns>
    /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
    /// </returns>
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        var name = binder.Name;
        Properties.TryGetValue(name, out result);

        return true;
    }

    /// <summary>
    /// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
    /// </summary>
    /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
    /// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, the <paramref name="value" /> is "Test".</param>
    /// <returns>
    /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
    /// </returns>
    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        var name = binder.Name;
        if (Properties.ContainsKey(name))
        {
            Properties[name] = value;
        }
        else
        {
            Properties.Add(name, value);
        }

        return true;
    }
}
