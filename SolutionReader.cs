using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;

namespace QuickOpenFile
{
    public class SolutionReader
    {
        List<SolutionFile> solutionFiles = new List<SolutionFile>();
        TraversalState traversalState = new TraversalState();
        Options options;

        public List<SolutionFile> GetSolutionFiles(IVsSolution solution, Options options)
        {
            //Get the solution service so we can traverse each project hierarchy contained within.
            traversalState.Clear();
            solutionFiles.Clear();
            //solutionFiles = new List<SolutionFile>();
            this.options = options;
            if (null != solution)
            {
                IVsHierarchy solutionHierarchy = solution as IVsHierarchy;
                if (null != solutionHierarchy)
                {
                    //OutputCommandString("\n\nTraverse All Items Recursively:\n");
                    EnumHierarchyItems(solutionHierarchy, VSConstants.VSITEMID_ROOT, 0, true, false);
                }
            }
            return solutionFiles;
        }

        /// <summary>
        /// Gets the item id.
        /// </summary>
        /// <param name="pvar">VARIANT holding an itemid.</param>
        /// <returns>Item Id of the concerned node</returns>
        private uint GetItemId(object pvar)
        {
            if (pvar == null) return VSConstants.VSITEMID_NIL;
            if (pvar is int) return (uint)(int)pvar;
            if (pvar is uint) return (uint)pvar;
            if (pvar is short) return (uint)(short)pvar;
            if (pvar is ushort) return (uint)(ushort)pvar;
            if (pvar is long) return (uint)(long)pvar;
            return VSConstants.VSITEMID_NIL;
        }

        /// <summary>
        /// Enumerates over the hierarchy items for the given hierarchy traversing into nested hierarchies.
        /// </summary>
        /// <param name="hierarchy">hierarchy to enmerate over.</param>
        /// <param name="itemid">item id of the hierarchy</param>
        /// <param name="recursionLevel">Depth of recursion. e.g. if recursion started with the Solution
        /// node, then : Level 0 -- Solution node, Level 1 -- children of Solution, etc.</param>
        /// <param name="hierIsSolution">true if hierarchy is Solution Node. This is needed to special
        /// case the children of the solution to work around a bug with VSHPROPID_FirstChild and 
        /// VSHPROPID_NextSibling implementation of the Solution.</param>
        /// <param name="visibleNodesOnly">true if only nodes visible in the Solution Explorer should
        /// be traversed. false if all project items should be traversed.</param>
        /// <param name="processNodeFunc">pointer to function that should be processed on each
        /// node as it is visited in the depth first enumeration.</param>
        private void EnumHierarchyItems(IVsHierarchy hierarchy, uint itemid, int recursionLevel, bool hierIsSolution, bool visibleNodesOnly)
        {
            int hr;
            IntPtr nestedHierarchyObj;
            uint nestedItemId;
            Guid hierGuid = typeof(IVsHierarchy).GUID;

            // Check first if this node has a nested hierarchy. If so, then there really are two 
            // identities for this node: 1. hierarchy/itemid 2. nestedHierarchy/nestedItemId.
            // We will recurse and call EnumHierarchyItems which will display this node using
            // the inner nestedHierarchy/nestedItemId identity.
            hr = hierarchy.GetNestedHierarchy(itemid, ref hierGuid, out nestedHierarchyObj, out nestedItemId);
            if (VSConstants.S_OK == hr && IntPtr.Zero != nestedHierarchyObj)
            {
                IVsHierarchy nestedHierarchy = Marshal.GetObjectForIUnknown(nestedHierarchyObj) as IVsHierarchy;
                Marshal.Release(nestedHierarchyObj);    // we are responsible to release the refcount on the out IntPtr parameter
                if (nestedHierarchy != null)
                {
                    // Display name and type of the node in the Output Window
                    EnumHierarchyItems(nestedHierarchy, nestedItemId, recursionLevel, false, visibleNodesOnly);
                }
            }
            else
            {
                object pVar;

                // Display name and type of the node in the Output Window
                ProcessSolutionNode(hierarchy, itemid, recursionLevel);

                recursionLevel++;

                //Get the first child node of the current hierarchy being walked
                // NOTE: to work around a bug with the Solution implementation of VSHPROPID_FirstChild,
                // we keep track of the recursion level. If we are asking for the first child under
                // the Solution, we use VSHPROPID_FirstVisibleChild instead of _FirstChild. 
                // In VS 2005 and earlier, the Solution improperly enumerates all nested projects
                // in the Solution (at any depth) as if they are immediate children of the Solution.
                // Its implementation _FirstVisibleChild is correct however, and given that there is
                // not a feature to hide a SolutionFolder or a Project, thus _FirstVisibleChild is 
                // expected to return the identical results as _FirstChild.
                hr = hierarchy.GetProperty(itemid,
                    ((visibleNodesOnly || (hierIsSolution && recursionLevel == 1) ?
                        (int)__VSHPROPID.VSHPROPID_FirstVisibleChild : (int)__VSHPROPID.VSHPROPID_FirstChild)),
                    out pVar);
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
                if (VSConstants.S_OK == hr)
                {
                    //We are using Depth first search so at each level we recurse to check if the node has any children
                    // and then look for siblings.
                    uint childId = GetItemId(pVar);
                    while (childId != VSConstants.VSITEMID_NIL)
                    {
                        EnumHierarchyItems(hierarchy, childId, recursionLevel, false, visibleNodesOnly);
                        // NOTE: to work around a bug with the Solution implementation of VSHPROPID_NextSibling,
                        // we keep track of the recursion level. If we are asking for the next sibling under
                        // the Solution, we use VSHPROPID_NextVisibleSibling instead of _NextSibling. 
                        // In VS 2005 and earlier, the Solution improperly enumerates all nested projects
                        // in the Solution (at any depth) as if they are immediate children of the Solution.
                        // Its implementation   _NextVisibleSibling is correct however, and given that there is
                        // not a feature to hide a SolutionFolder or a Project, thus _NextVisibleSibling is 
                        // expected to return the identical results as _NextSibling.
                        hr = hierarchy.GetProperty(childId,
                            ((visibleNodesOnly || (hierIsSolution && recursionLevel == 1)) ?
                                (int)__VSHPROPID.VSHPROPID_NextVisibleSibling : (int)__VSHPROPID.VSHPROPID_NextSibling),
                            out pVar);
                        if (VSConstants.S_OK == hr)
                        {
                            childId = GetItemId(pVar);
                        }
                        else
                        {
                            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This function diplays the name of the Hierarchy node. This function is passed to the 
        /// Hierarchy enumeration routines to process the current node.
        /// </summary>
        /// <param name="hierarchy">Hierarchy of the current node</param>
        /// <param name="itemid">Itemid of the current node</param>
        /// <param name="recursionLevel">Depth of recursion in hierarchy enumeration. We add one tab
        /// for each level in the recursion.</param>
        private void ProcessSolutionNode(IVsHierarchy hierarchy, uint itemid, int recursionLevel)
        {
            int hr;

            // get name
            object objName;
            hr = hierarchy.GetProperty(itemid, (int)__VSHPROPID.VSHPROPID_Name, out objName);

            if (recursionLevel == 0) traversalState.CurrentSolutionName = (string)objName;
            if (recursionLevel == 1) traversalState.CurrentProjectName = (string)objName;

            // skip non-member items (dependencies, files not referenced by the solution)
            object objIsNonMember;
            hr = hierarchy.GetProperty(itemid, (int)__VSHPROPID.VSHPROPID_IsNonMemberItem, out objIsNonMember);
            if (objIsNonMember != null)
                if ((bool)objIsNonMember)
                    return;

            SolutionFile sr = new SolutionFile();
            sr.Name = (string)objName;
            sr.Project = traversalState.CurrentProjectName;
            sr.ItemId = itemid;

            // get canonical filename and last write time
            if (recursionLevel > 0 && itemid != VSConstants.VSITEMID_NIL && itemid != VSConstants.VSITEMID_ROOT)
            {
                try
                {
                    string filePath = "";
                    if (hierarchy.GetCanonicalName(itemid, out filePath) == VSConstants.S_OK)
                    {
                        if (!string.IsNullOrEmpty(filePath) && System.IO.Path.IsPathRooted(filePath))
                        {
                            if (File.Exists(filePath))
                            {
                                sr.FilePath = filePath;
                                sr.LastWriteTime = File.GetLastWriteTime(filePath);
                            }
                        }
                    }
                }
                catch (ArgumentException) { }
                catch (System.Reflection.TargetInvocationException) { }
                catch (Exception) { }
            }

            //Debug.Print("QOF" + recursionLevel + "> " + sr.ItemId + ", " + sr.Name + ", " + sr.Project + ", " + sr.FilePath + ", " + sr.LastWriteTime);

            // ListViewItem is creation moved to the gui thread.
            //sr.CreateListViewItem();

            // Exclude empty names and paths (also non-existent files).
            if (string.IsNullOrEmpty(sr.Name) || string.IsNullOrEmpty(sr.FilePath))
                return;

            // Exclude canonical names that appear to be directories
            if (sr.FilePath.EndsWith("\\") || sr.FilePath.EndsWith("/"))
                return;

            // get icon
            object objIconIndex;
            hr = hierarchy.GetProperty(itemid, (int)__VSHPROPID.VSHPROPID_IconIndex, out objIconIndex);
            if (objIconIndex != null) sr.IconIndex = (int)objIconIndex;
            //TODO: look how to obtain item's icons for display in the list view
            //    -> http://connect.microsoft.com/VisualStudio/feedback/details/520256/cannot-find-icon-for-vs2010-database-project

            solutionFiles.Add(sr);
        }
    }
}
