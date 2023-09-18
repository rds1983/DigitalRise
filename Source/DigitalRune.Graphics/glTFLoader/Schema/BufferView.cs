//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace glTFLoader.Schema {
    using System.Linq;
    using System.Runtime.Serialization;
    
    
    public class BufferView {
        
        /// <summary>
        /// Backing field for Buffer.
        /// </summary>
        private int m_buffer;
        
        /// <summary>
        /// Backing field for ByteOffset.
        /// </summary>
        private int m_byteOffset = 0;
        
        /// <summary>
        /// Backing field for ByteLength.
        /// </summary>
        private int m_byteLength;
        
        /// <summary>
        /// Backing field for ByteStride.
        /// </summary>
        private System.Nullable<int> m_byteStride;
        
        /// <summary>
        /// Backing field for Target.
        /// </summary>
        private System.Nullable<TargetEnum> m_target;
        
        /// <summary>
        /// Backing field for Name.
        /// </summary>
        private string m_name;
        
        /// <summary>
        /// Backing field for Extensions.
        /// </summary>
        private System.Collections.Generic.Dictionary<string, object> m_extensions;
        
        /// <summary>
        /// Backing field for Extras.
        /// </summary>
        private Extras m_extras;
        
        /// <summary>
        /// The index of the buffer.
        /// </summary>
        [Newtonsoft.Json.JsonRequiredAttribute()]
        [Newtonsoft.Json.JsonPropertyAttribute("buffer")]
        public int Buffer {
            get {
                return this.m_buffer;
            }
            set {
                if ((value < 0)) {
                    throw new System.ArgumentOutOfRangeException("Buffer", value, "Expected value to be greater than or equal to 0");
                }
                this.m_buffer = value;
            }
        }
        
        /// <summary>
        /// The offset into the buffer in bytes.
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("byteOffset")]
        public int ByteOffset {
            get {
                return this.m_byteOffset;
            }
            set {
                if ((value < 0)) {
                    throw new System.ArgumentOutOfRangeException("ByteOffset", value, "Expected value to be greater than or equal to 0");
                }
                this.m_byteOffset = value;
            }
        }
        
        /// <summary>
        /// The length of the bufferView in bytes.
        /// </summary>
        [Newtonsoft.Json.JsonRequiredAttribute()]
        [Newtonsoft.Json.JsonPropertyAttribute("byteLength")]
        public int ByteLength {
            get {
                return this.m_byteLength;
            }
            set {
                if ((value < 1)) {
                    throw new System.ArgumentOutOfRangeException("ByteLength", value, "Expected value to be greater than or equal to 1");
                }
                this.m_byteLength = value;
            }
        }
        
        /// <summary>
        /// The stride, in bytes.
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("byteStride")]
        public System.Nullable<int> ByteStride {
            get {
                return this.m_byteStride;
            }
            set {
                if ((value < 4)) {
                    throw new System.ArgumentOutOfRangeException("ByteStride", value, "Expected value to be greater than or equal to 4");
                }
                if ((value > 252)) {
                    throw new System.ArgumentOutOfRangeException("ByteStride", value, "Expected value to be less than or equal to 252");
                }
                this.m_byteStride = value;
            }
        }
        
        /// <summary>
        /// The hint representing the intended GPU buffer type to use with this buffer view.
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("target")]
        public System.Nullable<TargetEnum> Target {
            get {
                return this.m_target;
            }
            set {
                this.m_target = value;
            }
        }
        
        /// <summary>
        /// The user-defined name of this object.
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("name")]
        public string Name {
            get {
                return this.m_name;
            }
            set {
                this.m_name = value;
            }
        }
        
        /// <summary>
        /// JSON object with extension-specific objects.
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("extensions")]
        public System.Collections.Generic.Dictionary<string, object> Extensions {
            get {
                return this.m_extensions;
            }
            set {
                this.m_extensions = value;
            }
        }
        
        /// <summary>
        /// Application-specific data.
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("extras")]
        public Extras Extras {
            get {
                return this.m_extras;
            }
            set {
                this.m_extras = value;
            }
        }
        
        public bool ShouldSerializeByteOffset() {
            return ((m_byteOffset == 0) 
                        == false);
        }
        
        public bool ShouldSerializeByteStride() {
            return ((m_byteStride == null) 
                        == false);
        }
        
        public bool ShouldSerializeTarget() {
            return ((m_target == null) 
                        == false);
        }
        
        public bool ShouldSerializeName() {
            return ((m_name == null) 
                        == false);
        }
        
        public bool ShouldSerializeExtensions() {
            return ((m_extensions == null) 
                        == false);
        }
        
        public bool ShouldSerializeExtras() {
            return ((m_extras == null) 
                        == false);
        }
        
        public enum TargetEnum {
            
            ARRAY_BUFFER = 34962,
            
            ELEMENT_ARRAY_BUFFER = 34963,
        }
    }
}
