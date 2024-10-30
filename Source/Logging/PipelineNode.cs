using System;
using System.Collections.Generic;
using System.Linq;

namespace Savage.Logs {

/// <summary> A node that exists in a <see cref="LogPipeline"/>, each node filters and attaches metadata to the <see cref="LogEntry"/> objects that pass through it. </summary>
public abstract class PipelineNode {
    
    internal readonly List<PipelineNode> Children = new List<PipelineNode>();
    
    /// <summary> All messages will be compared against these filters before being allowed to continue through this node in the <see cref="LogPipeline"/>. </summary>
    internal readonly List<Predicate<LogEntry>> DropFilters = new List<Predicate<LogEntry>>();
    
    /// <summary> All messages that pass through this node will have decorations attached to them using these generators. </summary>
    internal readonly List<AttachmentGenerator> DecorationGenerators = new List<AttachmentGenerator>();

    internal readonly List<(Predicate<LogEntry> Predicate, AttachmentGenerator Generator)> ConditionalGenerators = new List<(Predicate<LogEntry> Predicate, AttachmentGenerator Generator)>();

    #region Construction
        public PipelineNode DropLogsWhen(Predicate<LogEntry> dropWhenTrue) {
            DropFilters.Add(dropWhenTrue);
            return this;
        }
            
        public PipelineNode Attach(AttachmentGenerator generator) {
        #if DEBUG
            if (DecorationGenerators.Contains(generator))
                throw new ArgumentException($"Identical generator already exists for {generator}");
        #endif
            DecorationGenerators.Add(generator);

            return this;
        }

        public PipelineNode AttachWhen(Predicate<LogEntry> attachmentCondition, AttachmentGenerator attachmentGenerator) {
            ConditionalGenerators.Add((attachmentCondition, attachmentGenerator));
            return this;
        }
            
        /// <remarks> When compiled with DEBUG, registering the same logger multiple times will throw an <see cref="ArgumentException"/>. </remarks>
        /// <exception cref="ArgumentException"> Identical logger has already been registered. </exception>
        public PipelineNode WriteTo(PipelineNode logger) {
            Children.Add(logger);

            return this;
        }
    #endregion
    
    
    internal bool ShouldDrop(in LogEntry entry) {
        for (int i = 0; i < DropFilters.Count; ++i) {
            if (DropFilters[i](entry))
                return true;
        }
        return false;
    }
    
    internal void AttachMetaDataTo(ref LogEntry entry) {
        for (int i = 0; i < DecorationGenerators.Count; ++i) {
            var attachment = DecorationGenerators[i].CreateAttachmentFor(ref entry);
            entry.Attachments.Add(attachment);
        }

        for (int i = 0; i < ConditionalGenerators.Count; ++i) {
            if (ConditionalGenerators[i].Predicate(entry)) {
                var attachment = ConditionalGenerators[i].Generator.CreateAttachmentFor(ref entry);
                entry.Attachments.Add(attachment);
            }
        }
    }

    /// <summary> Pushes a message on through the pipeline, message may be dropped or have additional data attached to it as it passes through each node. </summary>
    internal void ProcessAndPushToChildrenRecursive(LogEntry entry) {
        if (ShouldDrop(entry))
            return;
        
        AttachMetaDataTo(ref entry);
        
        // add the ability for a node to strip metadata it doesn't want?

        if (this is ILogSink sink)
            sink.Write(entry);
        
        for(int i = 0; i < Children.Count; ++i) 
            Children[i].ProcessAndPushToChildrenRecursive(entry);
    }
    
    
#region Queries

    /// <summary> Returns true if this pipeline has ANY <see cref="AttachmentGenerator"/> matching or deriving from the desired type. </summary>
    internal bool HasAttachmentGenerator<T>() where T : AttachmentGenerator {
        for (int i = 0; i < DecorationGenerators.Count; ++i)
            if (typeof(T).IsAssignableFrom(DecorationGenerators[i].GetType()))
                return true;
        
        for(int i = 0; i < ConditionalGenerators.Count; ++i)
            if (typeof(T).IsAssignableFrom(ConditionalGenerators[i].Generator.GetType()))
                return true;
        
        return false;
    }

    /// <summary> Removes ALL <see cref="AttachmentGenerator"/>s matching or deriving from the desired type. </summary>
    /// <typeparam name="T"></typeparam>
    internal void RemoveAttachmentGenerator<T>() where T : AttachmentGenerator {
        for (int i = 0; i < DecorationGenerators.Count; ++i) {
            if (typeof(T).IsAssignableFrom(DecorationGenerators[i].GetType())) {
                DecorationGenerators.RemoveAt(i);
                return;
            }
        }

        for (int i = 0; i < ConditionalGenerators.Count; ++i) {
            if (typeof(T).IsAssignableFrom(ConditionalGenerators[i].Generator.GetType())) {
                ConditionalGenerators.RemoveAt(i);
                return;
            }
        }
    }

#endregion Queries
}

}