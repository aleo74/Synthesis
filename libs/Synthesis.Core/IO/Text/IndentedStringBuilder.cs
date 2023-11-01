using System.Diagnostics;
using System.Text;

namespace Synthesis.Core.IO.Text;

/// <summary>
/// Represents a string builder for creating indented strings.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public sealed class IndentedStringBuilder
{
    private readonly StringBuilder _builder = new();

    private int _indentLevel;

    /// <summary>
    /// Increases the current indentation level.
    /// </summary>
    /// <returns>The updated <see cref="IndentedStringBuilder"/> instance.</returns>
    public IndentedStringBuilder Indent()
    {
        _indentLevel++;
        return this;
    }

    /// <summary>
    /// Decreases the current indentation level.
    /// </summary>
    /// <returns>The updated <see cref="IndentedStringBuilder"/> instance.</returns>
    public IndentedStringBuilder UnIndent()
    {
        _indentLevel = Math.Max(_indentLevel - 1, 0);
        return this;
    }

    /// <summary>
    /// Appends the specified string to the builder.
    /// </summary>
    /// <param name="value">The string to append.</param>
    /// <returns>The updated <see cref="IndentedStringBuilder"/> instance.</returns>
    public IndentedStringBuilder Append(string value)
    {
        _builder.Append(value);
        return this;
    }

    /// <summary>
    /// Appends a formatted string to the builder using the specified arguments.
    /// </summary>
    /// <param name="value">The format string.</param>
    /// <param name="args">An array of objects to format and append.</param>
    /// <returns>The updated <see cref="IndentedStringBuilder"/> instance.</returns>
    public IndentedStringBuilder Append(string value, params object[] args)
    {
        _builder.Append(string.Format(value, args));
        return this;
    }

    /// <summary>
    /// Appends the specified string with the current level of indentation.
    /// </summary>
    /// <param name="value">The string to append.</param>
    /// <returns>The updated <see cref="IndentedStringBuilder"/> instance.</returns>
    public IndentedStringBuilder AppendIndented(string value)
    {
        _builder
            .Append(new string(' ', _indentLevel * 4))
            .Append(value);
        return this;
    }

    /// <summary>
    /// Appends a formatted string with the current level of indentation using the specified arguments.
    /// </summary>
    /// <param name="value">The format string.</param>
    /// <param name="args">An array of objects to format and append.</param>
    /// <returns>The updated <see cref="IndentedStringBuilder"/> instance.</returns>
    public IndentedStringBuilder AppendIndented(string value, params object[] args)
    {
        _builder
            .Append(new string(' ', _indentLevel * 4))
            .Append(string.Format(value, args));
        return this;
    }

    /// <summary>
    /// Appends a new line to the builder.
    /// </summary>
    /// <returns>The updated <see cref="IndentedStringBuilder"/> instance.</returns>
    public IndentedStringBuilder AppendLine()
    {
        _builder.AppendLine();
        return this;
    }

    /// <summary>
    /// Appends the specified string followed by a new line.
    /// </summary>
    /// <param name="value">The string to append.</param>
    /// <returns>The updated <see cref="IndentedStringBuilder"/> instance.</returns>
    public IndentedStringBuilder AppendLine(string value)
    {
        _builder.AppendLine(value);
        return this;
    }

    /// <summary>
    /// Appends a formatted string followed by a new line using the specified arguments.
    /// </summary>
    /// <param name="value">The format string.</param>
    /// <param name="args">An array of objects to format and append.</param>
    /// <returns>The updated <see cref="IndentedStringBuilder"/> instance.</returns>
    public IndentedStringBuilder AppendLine(string value, params object[] args)
    {
        _builder.AppendLine(string.Format(value, args));
        return this;
    }

    /// <summary>
    /// Appends the specified string with the current level of indentation followed by a new line.
    /// </summary>
    /// <param name="value">The string to append.</param>
    /// <returns>The updated <see cref="IndentedStringBuilder"/> instance.</returns>
    public IndentedStringBuilder AppendIndentedLine(string value)
    {
        _builder
            .Append(new string(' ', _indentLevel * 4))
            .AppendLine(value);
        return this;
    }

    /// <summary>
    /// Appends a formatted string with the current level of indentation followed by a new line using the specified arguments.
    /// </summary>
    /// <param name="value">The format string.</param>
    /// <param name="args">An array of objects to format and append.</param>
    /// <returns>The updated <see cref="IndentedStringBuilder"/> instance.</returns>
    public IndentedStringBuilder AppendIndentedLine(string value, params object[] args)
    {
        _builder
            .Append(new string(' ', _indentLevel * 4))
            .AppendLine(string.Format(value, args));
        return this;
    }

    /// <summary>
    /// Returns the string representation of the builder.
    /// </summary>
    /// <returns>The string that has been built using this instance.</returns>
    public override string ToString()
    {
        return _builder.ToString();
    }
}