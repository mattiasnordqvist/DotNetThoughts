using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Results;

/// <summary>
/// A class to represent the return value of a void operation.
/// There is no practical difference between two Unit instances, therefore only one instance is ever needed.
/// 
/// The static Instance property can be used to get the single instance of Unit.
/// 
/// Defined as a class instead of a struct to make sure no unnecessary copies are ever made.
/// </summary>
public class Unit
{
    /// <summary>
    /// The single instance of Unit.
    /// </summary>
    public static Unit Instance = new Unit();
    private Unit() { }
}
