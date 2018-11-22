using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ServerLogGrowthTracker.DomainModelGenerator
{

    /// <summary>
    /// This interface defines contracts for the Class that Implements custom domain model generation
    /// Implementation class will take two or more domain models as constructor parameter and it will
    /// implement the custom logic to merge/join/aggregate calculate new fields and return the final
    /// type List{T} or T based on the method calls.
    /// e.g. Generate List{ResultDomainObject} by taking List{InputOneDomainObject}
    /// and List{InputTwoDomainObject} and applying custom transformation/aggregation.
    /// In future if a new transformation for another set of domain class is required, new implementation
    /// following this contract should be made.
    /// </summary>
    public interface IDomainModelGenerator<T>
    {
        /// <summary>
        /// e.g. Generate List{ServerLogFactGrowthInfo} by taking List{IServerLogFileInfo}
        /// and List{IServerLogFactInfo} as constructor argument and applying custom transformation/aggregation.
        /// </summary>
        /// <returns>IList{T}</returns>
        IList<T> GenerateList();


        /// <summary>
        /// Generates the Sliced DataSet, Slicing/Grouping logic is 
        /// </summary>
        /// <returns>IList<IList{T}></IList></returns>
        IList<List<T>> GenerateSlicedList();

        /// <summary>
        /// e.g. Generate ResultDomainObject by taking InputOneDomainObject
        /// and InputTwoDomainObject as constructor argument and applying custom transformation/aggregation.
        /// </summary>
        /// <returns>T</returns>
        T Generate();
    }
}
