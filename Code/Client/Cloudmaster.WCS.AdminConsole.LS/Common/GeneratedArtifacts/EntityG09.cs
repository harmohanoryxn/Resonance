

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LightSwitchApplication
{
    #region Entities
    
    /// <summary>
    /// No Modeled Description Available
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.LightSwitch.BuildTasks.CodeGen", "10.0.0.0")]
    public sealed partial class Pin : global::Microsoft.LightSwitch.Framework.Base.EntityObject<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass>
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new instance of the Pin entity.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.LightSwitch.BuildTasks.CodeGen", "10.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public Pin()
            : this(null)
        {
        }
    
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.LightSwitch.BuildTasks.CodeGen", "10.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public Pin(global::Microsoft.LightSwitch.Framework.EntitySet<global::LightSwitchApplication.Pin> entitySet)
            : base(entitySet)
        {
            global::LightSwitchApplication.Pin.DetailsClass.Initialize(this);
        }
    
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        partial void Pin_Created();
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        partial void Pin_AllowSaveWithErrors(ref bool result);
    
        #endregion
    
        #region Private Properties
        
        /// <summary>
        /// Gets the Application object for this application.  The Application object provides access to active screens, methods to open screens and access to the current user.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.LightSwitch.BuildTasks.CodeGen", "10.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private global::Microsoft.LightSwitch.IApplication<global::LightSwitchApplication.DataWorkspace> Application
        {
            get
            {
                return global::LightSwitchApplication.Application.Current;
            }
        }
        
        /// <summary>
        /// Gets the containing data workspace.  The data workspace provides access to all data sources in the application.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.LightSwitch.BuildTasks.CodeGen", "10.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private global::LightSwitchApplication.DataWorkspace DataWorkspace
        {
            get
            {
                return (global::LightSwitchApplication.DataWorkspace)this.Details.EntitySet.Details.DataService.Details.DataWorkspace;
            }
        }
        
        #endregion
    
        #region Public Properties
    
        /// <summary>
        /// No Modeled Description Available
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.LightSwitch.BuildTasks.CodeGen", "10.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public int pinId
        {
            get
            {
                return global::LightSwitchApplication.Pin.DetailsClass.GetValue(this, global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties.pinId);
            }
        }
        
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        partial void pinId_IsReadOnly(ref bool result);
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        partial void pinId_Validate(global::Microsoft.LightSwitch.EntityValidationResultsBuilder results);
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        partial void pinId_Changed();

        /// <summary>
        /// No Modeled Description Available
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.LightSwitch.BuildTasks.CodeGen", "10.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public string pin1
        {
            get
            {
                return global::LightSwitchApplication.Pin.DetailsClass.GetValue(this, global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties.pin1);
            }
            set
            {
                global::LightSwitchApplication.Pin.DetailsClass.SetValue(this, global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties.pin1, value);
            }
        }
        
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        partial void pin1_IsReadOnly(ref bool result);
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        partial void pin1_Validate(global::Microsoft.LightSwitch.EntityValidationResultsBuilder results);
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        partial void pin1_Changed();

        /// <summary>
        /// No Modeled Description Available
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.LightSwitch.BuildTasks.CodeGen", "10.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::LightSwitchApplication.Device Device
        {
            get
            {
                return global::LightSwitchApplication.Pin.DetailsClass.GetValue(this, global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties.Device);
            }
            set
            {
                global::LightSwitchApplication.Pin.DetailsClass.SetValue(this, global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties.Device, value);
            }
        }
        
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        partial void Device_IsReadOnly(ref bool result);
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        partial void Device_Validate(global::Microsoft.LightSwitch.EntityValidationResultsBuilder results);
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        partial void Device_Changed();

        #endregion
    
        #region Details Class
    
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.LightSwitch.BuildTasks.CodeGen", "10.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public sealed class DetailsClass : global::Microsoft.LightSwitch.Details.Framework.Base.EntityDetails<
                global::LightSwitchApplication.Pin,
                global::LightSwitchApplication.Pin.DetailsClass,
                global::LightSwitchApplication.Pin.DetailsClass.IImplementation,
                global::LightSwitchApplication.Pin.DetailsClass.PropertySet,
                global::Microsoft.LightSwitch.Details.Framework.EntityCommandSet<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass>,
                global::Microsoft.LightSwitch.Details.Framework.EntityMethodSet<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass>>
        {
    
            static DetailsClass()
            {
                var initializeEntry = global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties.pinId;
            }
    
            [global::System.Diagnostics.DebuggerBrowsable(global::System.Diagnostics.DebuggerBrowsableState.Never)]
            private static readonly global::Microsoft.LightSwitch.Details.Framework.Base.EntityDetails<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass>.Entry
                __PinEntry = new global::Microsoft.LightSwitch.Details.Framework.Base.EntityDetails<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass>.Entry(
                    global::LightSwitchApplication.Pin.DetailsClass.__Pin_CreateNew,
                    global::LightSwitchApplication.Pin.DetailsClass.__Pin_Created,
                    global::LightSwitchApplication.Pin.DetailsClass.__Pin_AllowSaveWithErrors);
            private static global::LightSwitchApplication.Pin __Pin_CreateNew(global::Microsoft.LightSwitch.Framework.EntitySet<global::LightSwitchApplication.Pin> es)
            {
                return new global::LightSwitchApplication.Pin(es);
            }
            private static void __Pin_Created(global::LightSwitchApplication.Pin e)
            {
                e.Pin_Created();
            }
            private static bool __Pin_AllowSaveWithErrors(global::LightSwitchApplication.Pin e)
            {
                bool result = false;
                e.Pin_AllowSaveWithErrors(ref result);
                return result;
            }
    
            public DetailsClass() : base()
            {
            }
    
            public new global::Microsoft.LightSwitch.Details.Framework.EntityCommandSet<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass> Commands
            {
                get
                {
                    return base.Commands;
                }
            }
    
            public new global::Microsoft.LightSwitch.Details.Framework.EntityMethodSet<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass> Methods
            {
                get
                {
                    return base.Methods;
                }
            }
    
            public new global::LightSwitchApplication.Pin.DetailsClass.PropertySet Properties
            {
                get
                {
                    return base.Properties;
                }
            }
    
            [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.LightSwitch.BuildTasks.CodeGen", "10.0.0.0")]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public sealed class PropertySet : global::Microsoft.LightSwitch.Details.Framework.Base.EntityPropertySet<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass>
            {
    
                public PropertySet() : base()
                {
                }
    
                public global::Microsoft.LightSwitch.Details.Framework.EntityStorageProperty<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass, int> pinId
                {
                    get
                    {
                        return base.GetItem(global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties.pinId) as global::Microsoft.LightSwitch.Details.Framework.EntityStorageProperty<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass, int>;
                    }
                }
                
                public global::Microsoft.LightSwitch.Details.Framework.EntityStorageProperty<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass, string> pin1
                {
                    get
                    {
                        return base.GetItem(global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties.pin1) as global::Microsoft.LightSwitch.Details.Framework.EntityStorageProperty<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass, string>;
                    }
                }
                
                public global::Microsoft.LightSwitch.Details.Framework.EntityReferenceProperty<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass, global::LightSwitchApplication.Device> Device
                {
                    get
                    {
                        return base.GetItem(global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties.Device) as global::Microsoft.LightSwitch.Details.Framework.EntityReferenceProperty<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass, global::LightSwitchApplication.Device>;
                    }
                }
                
            }
    
            #pragma warning disable 109
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
            public interface IImplementation : global::Microsoft.LightSwitch.Internal.IEntityImplementation
            {
                new int pinId { get; }
                new string pin1 { get; set; }
                new global::Microsoft.LightSwitch.Internal.IEntityImplementation Device { get; set; }
            }
            #pragma warning restore 109
    
            [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.LightSwitch.BuildTasks.CodeGen", "10.0.0.0")]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            internal class PropertySetProperties
            {
    
                [global::System.Diagnostics.DebuggerBrowsable(global::System.Diagnostics.DebuggerBrowsableState.Never)]
                public static readonly global::Microsoft.LightSwitch.Details.Framework.EntityStorageProperty<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass, int>.Entry
                    pinId = new global::Microsoft.LightSwitch.Details.Framework.EntityStorageProperty<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass, int>.Entry(
                        "pinId",
                        global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties._pinId_Stub,
                        global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties._pinId_ComputeIsReadOnly,
                        global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties._pinId_Validate,
                        global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties._pinId_GetImplementationValue,
                        null,
                        global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties._pinId_OnValueChanged);
                private static void _pinId_Stub(global::Microsoft.LightSwitch.Details.Framework.Base.DetailsCallback<global::LightSwitchApplication.Pin.DetailsClass, global::Microsoft.LightSwitch.Details.Framework.EntityStorageProperty<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass, int>.Data> c, global::LightSwitchApplication.Pin.DetailsClass d, object sf)
                {
                    c(d, ref d._pinId, sf);
                }
                private static bool _pinId_ComputeIsReadOnly(global::LightSwitchApplication.Pin e)
                {
                    bool result = false;
                    e.pinId_IsReadOnly(ref result);
                    return result;
                }
                private static void _pinId_Validate(global::LightSwitchApplication.Pin e, global::Microsoft.LightSwitch.EntityValidationResultsBuilder r)
                {
                    e.pinId_Validate(r);
                }
                private static int _pinId_GetImplementationValue(global::LightSwitchApplication.Pin.DetailsClass d)
                {
                    return d.ImplementationEntity.pinId;
                }
                private static void _pinId_OnValueChanged(global::LightSwitchApplication.Pin e)
                {
                    e.pinId_Changed();
                }
    
                [global::System.Diagnostics.DebuggerBrowsable(global::System.Diagnostics.DebuggerBrowsableState.Never)]
                public static readonly global::Microsoft.LightSwitch.Details.Framework.EntityStorageProperty<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass, string>.Entry
                    pin1 = new global::Microsoft.LightSwitch.Details.Framework.EntityStorageProperty<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass, string>.Entry(
                        "pin1",
                        global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties._pin1_Stub,
                        global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties._pin1_ComputeIsReadOnly,
                        global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties._pin1_Validate,
                        global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties._pin1_GetImplementationValue,
                        global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties._pin1_SetImplementationValue,
                        global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties._pin1_OnValueChanged);
                private static void _pin1_Stub(global::Microsoft.LightSwitch.Details.Framework.Base.DetailsCallback<global::LightSwitchApplication.Pin.DetailsClass, global::Microsoft.LightSwitch.Details.Framework.EntityStorageProperty<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass, string>.Data> c, global::LightSwitchApplication.Pin.DetailsClass d, object sf)
                {
                    c(d, ref d._pin1, sf);
                }
                private static bool _pin1_ComputeIsReadOnly(global::LightSwitchApplication.Pin e)
                {
                    bool result = false;
                    e.pin1_IsReadOnly(ref result);
                    return result;
                }
                private static void _pin1_Validate(global::LightSwitchApplication.Pin e, global::Microsoft.LightSwitch.EntityValidationResultsBuilder r)
                {
                    e.pin1_Validate(r);
                }
                private static string _pin1_GetImplementationValue(global::LightSwitchApplication.Pin.DetailsClass d)
                {
                    return d.ImplementationEntity.pin1;
                }
                private static void _pin1_SetImplementationValue(global::LightSwitchApplication.Pin.DetailsClass d, string v)
                {
                    d.ImplementationEntity.pin1 = v;
                }
                private static void _pin1_OnValueChanged(global::LightSwitchApplication.Pin e)
                {
                    e.pin1_Changed();
                }
    
                [global::System.Diagnostics.DebuggerBrowsable(global::System.Diagnostics.DebuggerBrowsableState.Never)]
                public static readonly global::Microsoft.LightSwitch.Details.Framework.EntityReferenceProperty<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass, global::LightSwitchApplication.Device>.Entry
                    Device = new global::Microsoft.LightSwitch.Details.Framework.EntityReferenceProperty<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass, global::LightSwitchApplication.Device>.Entry(
                        "Device",
                        global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties._Device_Stub,
                        global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties._Device_ComputeIsReadOnly,
                        global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties._Device_Validate,
                        global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties._Device_GetCoreImplementationValue,
                        global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties._Device_GetImplementationValue,
                        global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties._Device_SetImplementationValue,
                        global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties._Device_Refresh,
                        global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties._Device_OnValueChanged);
                private static void _Device_Stub(global::Microsoft.LightSwitch.Details.Framework.Base.DetailsCallback<global::LightSwitchApplication.Pin.DetailsClass, global::Microsoft.LightSwitch.Details.Framework.EntityReferenceProperty<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass, global::LightSwitchApplication.Device>.Data> c, global::LightSwitchApplication.Pin.DetailsClass d, object sf)
                {
                    c(d, ref d._Device, sf);
                }
                private static bool _Device_ComputeIsReadOnly(global::LightSwitchApplication.Pin e)
                {
                    bool result = false;
                    e.Device_IsReadOnly(ref result);
                    return result;
                }
                private static void _Device_Validate(global::LightSwitchApplication.Pin e, global::Microsoft.LightSwitch.EntityValidationResultsBuilder r)
                {
                    e.Device_Validate(r);
                }
                private static global::Microsoft.LightSwitch.Internal.IEntityImplementation _Device_GetCoreImplementationValue(global::LightSwitchApplication.Pin.DetailsClass d)
                {
                    return d.ImplementationEntity.Device;
                }
                private static global::LightSwitchApplication.Device _Device_GetImplementationValue(global::LightSwitchApplication.Pin.DetailsClass d)
                {
                    return d.GetImplementationValue<global::LightSwitchApplication.Device, global::LightSwitchApplication.Device.DetailsClass>(global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties.Device, ref d._Device);
                }
                private static void _Device_SetImplementationValue(global::LightSwitchApplication.Pin.DetailsClass d, global::LightSwitchApplication.Device v)
                {
                    d.SetImplementationValue(global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties.Device, ref d._Device, (i, ev) => i.Device = ev, v);
                }
                private static void _Device_Refresh(global::LightSwitchApplication.Pin.DetailsClass d)
                {
                    d.RefreshNavigationProperty(global::LightSwitchApplication.Pin.DetailsClass.PropertySetProperties.Device, ref d._Device);
                }
                private static void _Device_OnValueChanged(global::LightSwitchApplication.Pin e)
                {
                    e.Device_Changed();
                }
    
            }
    
            [global::System.Diagnostics.DebuggerBrowsable(global::System.Diagnostics.DebuggerBrowsableState.Never)]
            private global::Microsoft.LightSwitch.Details.Framework.EntityStorageProperty<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass, int>.Data _pinId;
            
            [global::System.Diagnostics.DebuggerBrowsable(global::System.Diagnostics.DebuggerBrowsableState.Never)]
            private global::Microsoft.LightSwitch.Details.Framework.EntityStorageProperty<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass, string>.Data _pin1;
            
            [global::System.Diagnostics.DebuggerBrowsable(global::System.Diagnostics.DebuggerBrowsableState.Never)]
            private global::Microsoft.LightSwitch.Details.Framework.EntityReferenceProperty<global::LightSwitchApplication.Pin, global::LightSwitchApplication.Pin.DetailsClass, global::LightSwitchApplication.Device>.Data _Device;
            
        }
    
        #endregion
    }
    
    #endregion
}