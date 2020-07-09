// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using Talespin.Core.Foundation.Logging;

namespace Talespin.Core.Foundation.Misc
{
	/// <summary>
	/// Generic Tree implementation
	/// </summary>
	public class TreeNode<T> : IEnumerable<TreeNode<T>>
	{
		public readonly T Data;
		public TreeNode<T> Parent { get; private set; }
		public int ChildCount { get { return children.Count; } }
		private readonly List<TreeNode<T>> children = new List<TreeNode<T>>();

		public TreeNode<T> this[int idx]
		{
			get
			{
				if (idx < 0 || idx >= children.Count)
				{
					return null;
				}
				return children[idx];
			}
		}

		public TreeNode(T data)
		{
			Data = data;
		}

		public void AddChild(TreeNode<T> tree)
		{
			if (tree.Parent != null)
			{
				LogUtil.Error(LogTags.SYSTEM, this, "Tree node of type " + typeof(T) + " already has a parent. Possible cyclic referencing!");
			}
			else
			{
				tree.Parent = this;
			}
			children.Add(tree);
		}

		public void RemoveChild(TreeNode<T> tree)
		{
			children.Remove(tree);
		}

		public TreeNode<U> ConvertToTree<U>(System.Func<T, U> conversionFunc)
		{
			var tree = new TreeNode<U>(conversionFunc(Data));
			AddChildren(tree, this, conversionFunc);
			return tree;
		}

		public IEnumerator<TreeNode<T>> GetEnumerator()
		{
			return children.GetEnumerator();
		}

		public void ReplaceChildren(params TreeNode<T>[] newChildren)
		{
			children.Clear();
			children.AddRange(newChildren);
		}

		public TreeNode<T> Find(T data)
		{
			if (Data.Equals(data))
			{
				return this;
			}
			for (int i = 0; i < children.Count; i++)
			{
				TreeNode<T> child = children[i];
				TreeNode<T> ret = child.Find(data);
				if (ret != null)
				{
					return ret;
				}
			}
			return null;
		}

		private void AddChildren<U>(TreeNode<U> dest, TreeNode<T> src, System.Func<T, U> conversionFunc)
		{
			for (int i = 0; i < src.children.Count; i++)
			{
				TreeNode<T> child = src.children[i];
				var newChild = new TreeNode<U>(conversionFunc(child.Data));
				dest.AddChild(newChild);
				AddChildren(newChild, child, conversionFunc);
			}
		}

		// This is deliberately not private! will break if you add a modifier. 
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}