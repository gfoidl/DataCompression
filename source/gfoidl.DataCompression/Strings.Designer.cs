﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace gfoidl.DataCompression {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("gfoidl.DataCompression.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You have called &apos;MoveNext&apos; on the instance of &apos;DataPointIterator&apos; without getting the enumerator first. To get the enumerator call &apos;GetEnumerator&apos; on the &apos;DataPointIterator&apos; and use this instance.
        ///
        ///&apos;DataPointIterator&apos; is an enumerable and iterator in one. This saves an object on the heap, hence it is a performance optimization.
        ///When the &apos;DataPointIterator&apos; as enumerable is &quot;fresh&quot;, i.e. its state is as initialized, a call to &apos;GetEnumerator&apos; will just return the same object. Only when the (internal) state [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GetEnumerator_must_be_called_first {
            get {
                return ResourceManager.GetString("GetEnumerator_must_be_called_first", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Datapoints a and b must not be equal for calculation of the gradient.
        /// </summary>
        internal static string Gradient_A_eq_B {
            get {
                return ResourceManager.GetString("Gradient_A_eq_B", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This state should not happen. Please file a bug in https://github.com/gfoidl/DataCompression/issues/new 
        ///
        ///Thank you!.
        /// </summary>
        internal static string Should_not_happen {
            get {
                return ResourceManager.GetString("Should_not_happen", resourceCulture);
            }
        }
    }
}
