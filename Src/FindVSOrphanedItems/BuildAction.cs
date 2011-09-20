namespace Tools.VisualStudioOrphanedFiles
{
    /// <summary>
    /// Specifies the build action of an item in a project.
    /// </summary>
    public enum BuildAction
    {
        /// <summary>
        /// No action.
        /// </summary>
        None,
        
        /// <summary>
        /// Compiled.
        /// </summary>
        Compile,
        
        /// <summary>
        /// Linked
        /// </summary>
        Link,
        
        /// <summary>
        /// Contents.
        /// </summary>
        Content,
        
        /// <summary>
        /// Embedded resource.
        /// </summary>
        EmbeddedResource
    }
}