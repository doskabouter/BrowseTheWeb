// --------------------------------------------------------------------------------------------
// Version: MPL 1.1/GPL 2.0/LGPL 2.1
// 
// The contents of this file are subject to the Mozilla Public License Version
// 1.1 (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at
// http://www.mozilla.org/MPL/
// 
// Software distributed under the License is distributed on an "AS IS" basis,
// WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
// for the specific language governing rights and limitations under the
// License.
// 
// <remarks>
// Generated by IDLImporter from file nsICryptoHash.idl
// 
// You should use these interfaces when you access the COM objects defined in the mentioned
// IDL/IDH file.
// </remarks>
// --------------------------------------------------------------------------------------------
namespace Gecko
{
	using System;
	using System.Runtime.InteropServices;
	using System.Runtime.InteropServices.ComTypes;
	using System.Runtime.CompilerServices;
	using System.Windows.Forms;
	
	
	/// <summary>
    /// nsICryptoHash
    /// This interface provides crytographic hashing algorithms.
    /// </summary>
	[ComImport()]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("1e5b7c43-4688-45ce-92e1-77ed931e3bbe")]
	public interface nsICryptoHash
	{
		
		/// <summary>
        /// Initialize the hashing object. This method may be
        /// called multiple times with different algorithm types.
        ///
        /// @param aAlgorithm the algorithm type to be used.
        /// This value must be one of the above valid
        /// algorithm types.
        ///
        /// @throws NS_ERROR_INVALID_ARG if an unsupported algorithm
        /// type is passed.
        ///
        /// NOTE: This method or initWithString must be called
        /// before any other method on this interface is called.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void Init(uint aAlgorithm);
		
		/// <summary>
        /// Initialize the hashing object. This method may be
        /// called multiple times with different algorithm types.
        ///
        /// @param aAlgorithm the algorithm type to be used.
        ///
        /// @throws NS_ERROR_INVALID_ARG if an unsupported algorithm
        /// type is passed.
        ///
        /// NOTE: This method or init must be called before any
        /// other method on this interface is called.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void InitWithString([MarshalAs(UnmanagedType.LPStruct)] nsACStringBase aAlgorithm);
		
		/// <summary>
        /// @param aData a buffer to calculate the hash over
        ///
        /// @param aLen the length of the buffer |aData|
        ///
        /// @throws NS_ERROR_NOT_INITIALIZED if |init| has not been
        /// called.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void Update([MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] byte[] aData, uint aLen);
		
		/// <summary>
        /// Calculates and updates a new hash based on a given data stream.
        ///
        /// @param aStream an input stream to read from.
        ///
        /// @param aLen how much to read from the given |aStream|.  Passing
        /// PR_UINT32_MAX indicates that all data available will be used
        /// to update the hash.
        ///
        /// @throws NS_ERROR_NOT_INITIALIZED if |init| has not been
        /// called.
        ///
        /// @throws NS_ERROR_NOT_AVAILABLE if the requested amount of
        /// data to be calculated into the hash is not available.
        ///
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void UpdateFromStream([MarshalAs(UnmanagedType.Interface)] nsIInputStream aStream, uint aLen);
		
		/// <summary>
        /// Completes this hash object and produces the actual hash data.
        ///
        /// @param aASCII if true then the returned value is a base-64
        /// encoded string.  if false, then the returned value is
        /// binary data.
        ///
        /// @return a hash of the data that was read by this object.  This can
        /// be either binary data or base 64 encoded.
        ///
        /// @throws NS_ERROR_NOT_INITIALIZED if |init| has not been
        /// called.
        ///
        /// NOTE: This method may be called any time after |init|
        /// is called.  This call resets the object to its
        /// pre-init state.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void Finish([MarshalAs(UnmanagedType.U1)] bool aASCII, [MarshalAs(UnmanagedType.LPStruct)] nsACStringBase retval);
	}
	
	/// <summary>nsICryptoHashConsts </summary>
	public class nsICryptoHashConsts
	{
		
		// <summary>
        // Hashing Algorithms.  These values are to be used by the
        // |init| method to indicate which hashing function to
        // use.  These values map directly onto the values defined
        // in mozilla/security/nss/lib/cryptohi/hasht.h.
        // </summary>
		public const int MD2 = 1;
		
		// <summary>
        //String value: "md2" </summary>
		public const int MD5 = 2;
		
		// <summary>
        //String value: "md5" </summary>
		public const int SHA1 = 3;
		
		// <summary>
        //String value: "sha1" </summary>
		public const int SHA256 = 4;
		
		// <summary>
        //String value: "sha256" </summary>
		public const int SHA384 = 5;
		
		// <summary>
        //String value: "sha384" </summary>
		public const int SHA512 = 6;
	}
}
