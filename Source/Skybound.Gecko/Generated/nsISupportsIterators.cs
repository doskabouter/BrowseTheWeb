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
// Generated by IDLImporter from file nsISupportsIterators.idl
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
    /// ...
    /// </summary>
	[ComImport()]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("7330650e-1dd2-11b2-a0c2-9ff86ee97bed")]
	public interface nsIOutputIterator
	{
		
		/// <summary>
        /// Put |anElementToPut| into the underlying container or sequence at the position currently pointed to by this iterator.
        /// The iterator and the underlying container or sequence cooperate to |Release()|
        /// the replaced element, if any and if necessary, and to |AddRef()| the new element.
        ///
        /// The result is undefined if this iterator currently points outside the
        /// useful range of the underlying container or sequence.
        ///
        /// @param anElementToPut the element to place into the underlying container or sequence
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void PutElement([MarshalAs(UnmanagedType.Interface)] nsISupports anElementToPut);
		
		/// <summary>
        /// Advance this iterator to the next position in the underlying container or sequence.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void StepForward();
	}
	
	/// <summary>
    /// ...
    /// </summary>
	[ComImport()]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("85585e12-1dd2-11b2-a930-f6929058269a")]
	public interface nsIInputIterator
	{
		
		/// <summary>
        /// Retrieve (and |AddRef()|) the element this iterator currently points to.
        ///
        /// The result is undefined if this iterator currently points outside the
        /// useful range of the underlying container or sequence.
        ///
        /// @result a new reference to the element this iterator currently points to (if any)
        /// </summary>
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		nsISupports GetElement();
		
		/// <summary>
        /// Advance this iterator to the next position in the underlying container or sequence.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void StepForward();
		
		/// <summary>
        /// Test if |anotherIterator| points to the same position in the underlying container or sequence.
        ///
        /// The result is undefined if |anotherIterator| was not created by or for the same underlying container or sequence.
        ///
        /// @param anotherIterator another iterator to compare against, created by or for the same underlying container or sequence
        /// @result true if |anotherIterator| points to the same position in the underlying container or sequence
        /// </summary>
		[return: MarshalAs(UnmanagedType.U1)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		bool IsEqualTo([MarshalAs(UnmanagedType.Interface)] nsISupports anotherIterator);
		
		/// <summary>
        /// Create a new iterator pointing to the same position in the underlying container or sequence to which this iterator currently points.
        /// The returned iterator is suitable for use in a subsequent call to |isEqualTo()| against this iterator.
        ///
        /// @result a new iterator pointing at the same position in the same underlying container or sequence as this iterator
        /// </summary>
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		nsISupports Clone();
	}
	
	/// <summary>
    /// ...
    /// </summary>
	[ComImport()]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("8da01646-1dd2-11b2-98a7-c7009045be7e")]
	public interface nsIForwardIterator
	{
		
		/// <summary>
        /// Retrieve (and |AddRef()|) the element this iterator currently points to.
        ///
        /// The result is undefined if this iterator currently points outside the
        /// useful range of the underlying container or sequence.
        ///
        /// @result a new reference to the element this iterator currently points to (if any)
        /// </summary>
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		nsISupports GetElement();
		
		/// <summary>
        /// Put |anElementToPut| into the underlying container or sequence at the position currently pointed to by this iterator.
        /// The iterator and the underlying container or sequence cooperate to |Release()|
        /// the replaced element, if any and if necessary, and to |AddRef()| the new element.
        ///
        /// The result is undefined if this iterator currently points outside the
        /// useful range of the underlying container or sequence.
        ///
        /// @param anElementToPut the element to place into the underlying container or sequence
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void PutElement([MarshalAs(UnmanagedType.Interface)] nsISupports anElementToPut);
		
		/// <summary>
        /// Advance this iterator to the next position in the underlying container or sequence.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void StepForward();
		
		/// <summary>
        /// Test if |anotherIterator| points to the same position in the underlying container or sequence.
        ///
        /// The result is undefined if |anotherIterator| was not created by or for the same underlying container or sequence.
        ///
        /// @param anotherIterator another iterator to compare against, created by or for the same underlying container or sequence
        /// @result true if |anotherIterator| points to the same position in the underlying container or sequence
        /// </summary>
		[return: MarshalAs(UnmanagedType.U1)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		bool IsEqualTo([MarshalAs(UnmanagedType.Interface)] nsISupports anotherIterator);
		
		/// <summary>
        /// Create a new iterator pointing to the same position in the underlying container or sequence to which this iterator currently points.
        /// The returned iterator is suitable for use in a subsequent call to |isEqualTo()| against this iterator.
        ///
        /// @result a new iterator pointing at the same position in the same underlying container or sequence as this iterator
        /// </summary>
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		nsISupports Clone();
	}
	
	/// <summary>
    /// ...
    /// </summary>
	[ComImport()]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("948defaa-1dd1-11b2-89f6-8ce81f5ebda9")]
	public interface nsIBidirectionalIterator
	{
		
		/// <summary>
        /// Retrieve (and |AddRef()|) the element this iterator currently points to.
        ///
        /// The result is undefined if this iterator currently points outside the
        /// useful range of the underlying container or sequence.
        ///
        /// @result a new reference to the element this iterator currently points to (if any)
        /// </summary>
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		nsISupports GetElement();
		
		/// <summary>
        /// Put |anElementToPut| into the underlying container or sequence at the position currently pointed to by this iterator.
        /// The iterator and the underlying container or sequence cooperate to |Release()|
        /// the replaced element, if any and if necessary, and to |AddRef()| the new element.
        ///
        /// The result is undefined if this iterator currently points outside the
        /// useful range of the underlying container or sequence.
        ///
        /// @param anElementToPut the element to place into the underlying container or sequence
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void PutElement([MarshalAs(UnmanagedType.Interface)] nsISupports anElementToPut);
		
		/// <summary>
        /// Advance this iterator to the next position in the underlying container or sequence.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void StepForward();
		
		/// <summary>
        /// Move this iterator to the previous position in the underlying container or sequence.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void StepBackward();
		
		/// <summary>
        /// Test if |anotherIterator| points to the same position in the underlying container or sequence.
        ///
        /// The result is undefined if |anotherIterator| was not created by or for the same underlying container or sequence.
        ///
        /// @param anotherIterator another iterator to compare against, created by or for the same underlying container or sequence
        /// @result true if |anotherIterator| points to the same position in the underlying container or sequence
        /// </summary>
		[return: MarshalAs(UnmanagedType.U1)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		bool IsEqualTo([MarshalAs(UnmanagedType.Interface)] nsISupports anotherIterator);
		
		/// <summary>
        /// Create a new iterator pointing to the same position in the underlying container or sequence to which this iterator currently points.
        /// The returned iterator is suitable for use in a subsequent call to |isEqualTo()| against this iterator.
        ///
        /// @result a new iterator pointing at the same position in the same underlying container or sequence as this iterator
        /// </summary>
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		nsISupports Clone();
	}
	
	/// <summary>
    /// ...
    /// </summary>
	[ComImport()]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("9bd6fdb0-1dd1-11b2-9101-d15375968230")]
	public interface nsIRandomAccessIterator
	{
		
		/// <summary>
        /// Retrieve (and |AddRef()|) the element this iterator currently points to.
        ///
        /// The result is undefined if this iterator currently points outside the
        /// useful range of the underlying container or sequence.
        ///
        /// @result a new reference to the element this iterator currently points to (if any)
        /// </summary>
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		nsISupports GetElement();
		
		/// <summary>
        /// Retrieve (and |AddRef()|) an element at some offset from where this iterator currently points.
        /// The offset may be negative.  |getElementAt(0)| is equivalent to |getElement()|.
        ///
        /// The result is undefined if this iterator currently points outside the
        /// useful range of the underlying container or sequence.
        ///
        /// @param anOffset a |0|-based offset from the position to which this iterator currently points
        /// @result a new reference to the indicated element (if any)
        /// </summary>
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		nsISupports GetElementAt(int anOffset);
		
		/// <summary>
        /// Put |anElementToPut| into the underlying container or sequence at the position currently pointed to by this iterator.
        /// The iterator and the underlying container or sequence cooperate to |Release()|
        /// the replaced element, if any and if necessary, and to |AddRef()| the new element.
        ///
        /// The result is undefined if this iterator currently points outside the
        /// useful range of the underlying container or sequence.
        ///
        /// @param anElementToPut the element to place into the underlying container or sequence
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void PutElement([MarshalAs(UnmanagedType.Interface)] nsISupports anElementToPut);
		
		/// <summary>
        /// Put |anElementToPut| into the underlying container or sequence at the position |anOffset| away from that currently pointed to by this iterator.
        /// The iterator and the underlying container or sequence cooperate to |Release()|
        /// the replaced element, if any and if necessary, and to |AddRef()| the new element.
        /// |putElementAt(0, obj)| is equivalent to |putElement(obj)|.
        ///
        /// The result is undefined if this iterator currently points outside the
        /// useful range of the underlying container or sequence.
        ///
        /// @param anOffset a |0|-based offset from the position to which this iterator currently points
        /// @param anElementToPut the element to place into the underlying container or sequence
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void PutElementAt(int anOffset, [MarshalAs(UnmanagedType.Interface)] nsISupports anElementToPut);
		
		/// <summary>
        /// Advance this iterator to the next position in the underlying container or sequence.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void StepForward();
		
		/// <summary>
        /// Move this iterator by |anOffset| positions in the underlying container or sequence.
        /// |anOffset| may be negative.  |stepForwardBy(1)| is equivalent to |stepForward()|.
        /// |stepForwardBy(0)| is a no-op.
        ///
        /// @param anOffset a |0|-based offset from the position to which this iterator currently points
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void StepForwardBy(int anOffset);
		
		/// <summary>
        /// Move this iterator to the previous position in the underlying container or sequence.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void StepBackward();
		
		/// <summary>
        /// Move this iterator backwards by |anOffset| positions in the underlying container or sequence.
        /// |anOffset| may be negative.  |stepBackwardBy(1)| is equivalent to |stepBackward()|.
        /// |stepBackwardBy(n)| is equivalent to |stepForwardBy(-n)|.  |stepBackwardBy(0)| is a no-op.
        ///
        /// @param anOffset a |0|-based offset from the position to which this iterator currently points
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void StepBackwardBy(int anOffset);
		
		/// <summary>
        /// Test if |anotherIterator| points to the same position in the underlying container or sequence.
        ///
        /// The result is undefined if |anotherIterator| was not created by or for the same underlying container or sequence.
        ///
        /// @param anotherIterator another iterator to compare against, created by or for the same underlying container or sequence
        /// @result true if |anotherIterator| points to the same position in the underlying container or sequence
        /// </summary>
		[return: MarshalAs(UnmanagedType.U1)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		bool IsEqualTo([MarshalAs(UnmanagedType.Interface)] nsISupports anotherIterator);
		
		/// <summary>
        /// Create a new iterator pointing to the same position in the underlying container or sequence to which this iterator currently points.
        /// The returned iterator is suitable for use in a subsequent call to |isEqualTo()| against this iterator.
        ///
        /// @result a new iterator pointing at the same position in the same underlying container or sequence as this iterator
        /// </summary>
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		nsISupports Clone();
	}
}
