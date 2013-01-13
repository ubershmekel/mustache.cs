﻿using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace mustache.test
{
    /// <summary>
    /// Tests the FormatParser class.
    /// </summary>
    [TestClass]
    public class FormatCompilerTester
    {
        #region Tagless Formats

        /// <summary>
        /// If the given format is null, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCompile_NullFormat_Throws()
        {
            FormatCompiler compiler = new FormatCompiler();
            compiler.Compile(null);
        }

        /// <summary>
        /// If the format string contains no tag, then the given format string
        /// should be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_NoTags_PrintsFormatString()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = "This is an ordinary string.";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(null);
            Assert.AreEqual(format, result, "The generated text was wrong.");
        }

        /// <summary>
        /// If a line is just whitespace, it should be printed out as is.
        /// </summary>
        [TestMethod]
        public void TestCompile_LineAllWhitespace_PrintsWhitespace()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = "\t    \t";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(null);
            Assert.AreEqual(format, result, "The generated text was wrong.");
        }

        /// <summary>
        /// If a line has output, then the next line is blank, then both lines
        /// should be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_OutputNewLineBlank_PrintsBothLines()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"Hello
    ";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(null);
            Assert.AreEqual(format, result, "The wrong text was generated.");
        }

        #endregion

        #region Key

        /// <summary>
        /// Replaces placeholds with the actual value.
        /// </summary>
        [TestMethod]
        public void TestCompile_Key_ReplacesWithValue()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"Hello, {{Name}}!!!";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new { Name = "Bob" });
            Assert.AreEqual("Hello, Bob!!!", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If we pass null as the source object and the format string contains "this",
        /// then nothing should be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_ThisIsNull_PrintsNothing()
        {
            FormatCompiler compiler = new FormatCompiler();
            Generator generator = compiler.Compile("{{this}}");
            string result = generator.Render(null);
            Assert.AreEqual(String.Empty, result, "The wrong text was generated.");
        }

        /// <summary>
        /// If we try to print a key that doesn't exist, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestCompile_MissingKey_Throws()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"Hello, {{Name}}!!!";
            Generator generator = compiler.Compile(format);
            generator.Render(new object());
        }

        /// <summary>
        /// If we specify an alignment with a key, the alignment should
        /// be used when rending the value.
        /// </summary>
        [TestMethod]
        public void TestCompile_KeyWithNegativeAlignment_AppliesAlignment()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"Hello, {{Name,-10}}!!!";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new { Name = "Bob" });
            Assert.AreEqual("Hello, Bob       !!!", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If we specify an alignment with a key, the alignment should
        /// be used when rending the value.
        /// </summary>
        [TestMethod]
        public void TestCompile_KeyWithPositiveAlignment_AppliesAlignment()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"Hello, {{Name,10}}!!!";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new { Name = "Bob" });
            Assert.AreEqual("Hello,        Bob!!!", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If we specify a positive alignment with a key with an optional + character, 
        /// the alignment should be used when rending the value.
        /// </summary>
        [TestMethod]
        public void TestCompile_KeyWithPositiveAlignment_OptionalPlus_AppliesAlignment()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"Hello, {{Name,+10}}!!!";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new { Name = "Bob" });
            Assert.AreEqual("Hello,        Bob!!!", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If we specify an format with a key, the format should
        /// be used when rending the value.
        /// </summary>
        [TestMethod]
        public void TestCompile_KeyWithFormat_AppliesFormatting()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"Hello, {{When:yyyyMMdd}}!!!";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new { When = new DateTime(2012, 01, 31) });
            Assert.AreEqual("Hello, 20120131!!!", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If we specify an alignment with a key, the alignment should
        /// be used when rending the value.
        /// </summary>
        [TestMethod]
        public void TestCompile_KeyWithAlignmentAndFormat_AppliesBoth()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"Hello, {{When,10:yyyyMMdd}}!!!";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new { When = new DateTime(2012, 01, 31) });
            Assert.AreEqual("Hello,   20120131!!!", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If we dot separate keys, the value will be found by searching
        /// through the properties.
        /// </summary>
        [TestMethod]
        public void TestCompile_NestedKeys_NestedProperties()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"Hello, {{Top.Middle.Bottom}}!!!";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new { Top = new { Middle = new { Bottom = "Bob" } } });
            Assert.AreEqual("Hello, Bob!!!", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If a line has output, then the next line is blank, then both lines
        /// should be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_OutputNewLineOutput_PrintsBothLines()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"{{this}}
After";
            Generator generator = compiler.Compile(format);
            string result = generator.Render("Content");
            const string expected = @"Content
After";
            Assert.AreEqual(expected, result, "The wrong text was generated.");
        }

        /// <summary>
        /// If there is a line followed by a line with a key, both lines should be
        /// printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_EmptyNewLineKey_PrintsBothLines()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"
{{this}}";
            Generator generator = compiler.Compile(format);
            string result = generator.Render("Content");
            const string expected = @"
Content";
            Assert.AreEqual(expected, result, "The wrong text was generated.");
        }

        /// <summary>
        /// If there is a no-output line followed by a line with a key, the first line
        /// should be removed.
        /// </summary>
        [TestMethod]
        public void TestCompile_NoOutputNewLineKey_PrintsBothLines()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"{{#! comment }}
{{this}}";
            Generator generator = compiler.Compile(format);
            string result = generator.Render("Content");
            const string expected = @"Content";
            Assert.AreEqual(expected, result, "The wrong text was generated.");
        }

        /// <summary>
        /// If there is a comment on one line followed by a line with a key, the first line
        /// should be removed.
        /// </summary>
        [TestMethod]
        public void TestCompile_KeyKey_PrintsBothLines()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"{{this}}
{{this}}";
            Generator generator = compiler.Compile(format);
            string result = generator.Render("Content");
            const string expected = @"Content
Content";
            Assert.AreEqual(expected, result, "The wrong text was generated.");
        }

        #endregion

        #region Comment

        /// <summary>
        /// Removes comments from the middle of text.
        /// </summary>
        [TestMethod]
        public void TestCompile_Comment_RemovesComment()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = "Before{{#! This is a comment }}After";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new object());
            Assert.AreEqual("BeforeAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// Removes comments surrounding text.
        /// </summary>
        [TestMethod]
        public void TestCompile_CommentContentComment_RemovesComment()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = "{{#! comment }}Middle{{#! comment }}";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new object());
            Assert.AreEqual("Middle", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If blank space is surrounded by comments, the line should be removed.
        /// </summary>
        [TestMethod]
        public void TestCompile_CommentBlankComment_RemovesLine()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = "{{#! comment }}    {{#! comment }}";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new object());
            Assert.AreEqual(String.Empty, result, "The wrong text was generated.");
        }

        /// <summary>
        /// If a comment follows text, the comment should be removed.
        /// </summary>
        [TestMethod]
        public void TestCompile_ContentComment_RemovesComment()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = "Front{{#! comment }}";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new object());
            Assert.AreEqual("Front", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If a comment follows text, the comment should be removed.
        /// </summary>
        [TestMethod]
        public void TestCompile_ContentCommentContentComment_RemovesComments()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = "Front{{#! comment }}Middle{{#! comment }}";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new object());
            Assert.AreEqual("FrontMiddle", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If a comment makes up the entire format string, the nothing should be printed out.
        /// </summary>
        [TestMethod]
        public void TestCompile_CommentAloneOnlyLine__PrintsEmpty()
        {
            FormatCompiler compiler = new FormatCompiler();
            Generator generator = compiler.Compile("    {{#! comment }}    ");
            string result = generator.Render(null);
            Assert.AreEqual(String.Empty, result, "The wrong text was generated.");
        }

        /// <summary>
        /// If a comment is on a line by itself, irrespective of leading or trailing whitespace,
        /// the line should be removed from output.
        /// </summary>
        [TestMethod]
        public void TestCompile_ContentNewLineCommentNewLineContent_RemovesLine()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"Before
    {{#! This is a comment }}    
After";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new object());
            const string expected = @"Before
After";
            Assert.AreEqual(expected, result, "The wrong text was generated.");
        }

        /// <summary>
        /// If multiple comments are on a line by themselves, irrespective of whitespace,
        /// the line should be removed from output.
        /// </summary>
        [TestMethod]
        public void TestCompile_ContentNewLineCommentCommentNewLineContent_RemovesLine()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"Before
    {{#! This is a comment }}    {{#! This is another comment }}    
After";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new object());
            const string expected = @"Before
After";
            Assert.AreEqual(expected, result, "The wrong text was generated.");
        }

        /// <summary>
        /// If comments are on a multiple lines by themselves, irrespective of whitespace,
        /// the lines should be removed from output.
        /// </summary>
        [TestMethod]
        public void TestCompile_CommentsOnMultipleLines_RemovesLines()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"Before
    {{#! This is a comment }}    
    {{#! This is another comment }}    

    {{#! This is the final comment }}
After";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new object());
            const string expected = @"Before

After";
            Assert.AreEqual(expected, result, "The wrong text was generated.");
        }

        /// <summary>
        /// If a comment is followed by text, the line should be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_ContentNewLineCommentContentNewLineContent_PrintsLine()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"Before
    {{#! This is a comment }}Extra
After";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new object());
            const string expected = @"Before
    Extra
After";
            Assert.AreEqual(expected, result, "The wrong text was generated.");
        }

        /// <summary>
        /// If a comment is followed by the last line in a format string,
        /// the comment line should be eliminated and the last line printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_CommentNewLineBlank_PrintsBlank()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"    {{#! comment }}
        ";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(null);
            Assert.AreEqual("        ", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If a comment is followed by the last line in a format string,
        /// the comment line should be eliminated and the last line printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_CommentNewLineContent_PrintsContent()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"    {{#! comment }}
After";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(null);
            Assert.AreEqual("After", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If a line with content is followed by a line with a comment, the first line should
        /// be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_ContentNewLineComment_PrintsContent()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"First
{{#! comment }}";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(null);
            Assert.AreEqual("First", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If a line has a comment, followed by line with content, followed by a line with a comment, only
        /// the content should be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_CommentNewLineContentNewLineComment_PrintsContent()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"{{#! comment }}
First
{{#! comment }}";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(null);
            Assert.AreEqual("First", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If there are lines with content, then a comment, then content, then a comment, only
        /// the content should be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_ContentNewLineCommentNewLineContentNewLineComment_PrintsContent()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"First
    {{#! comment }}
Middle
{{#! comment }}";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(null);
            const string expected = @"First
Middle";
            Assert.AreEqual(expected, result, "The wrong text was generated.");
        }

        /// <summary>
        /// If there is content and a comment on a line, followed by a comment,
        /// only the content should be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_ContentCommentNewLineComment_PrintsContent()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"First{{#! comment }}
{{#! comment }}";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(null);
            Assert.AreEqual("First", result, "The wrong text was generated.");
        }

        #endregion

        #region If

        /// <summary>
        /// If the condition evaluates to false, the content of an if statement should not be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_If_EvaluatesToFalse_SkipsContent()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if this}}Content{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(false);
            Assert.AreEqual("BeforeAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the condition evaluates to false, the content of an if statement should not be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_If_EvaluatesToTrue_PrintsContent()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if this}}Content{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(true);
            Assert.AreEqual("BeforeContentAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the header and footer appear on lines by themselves, they should not generate new lines.
        /// </summary>
        [TestMethod]
        public void TestCompile_IfNewLineContentNewLineEndIf_PrintsContent()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = @"{{#if this}}
Content
{{/if}}";
            Generator generator = parser.Compile(format);
            string result = generator.Render(true);
            Assert.AreEqual("Content", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the header and footer appear on lines by themselves, they should not generate new lines.
        /// </summary>
        [TestMethod]
        public void TestCompile_IfNewLineEndIf_PrintsNothing()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = @"{{#if this}}
{{/if}}";
            Generator generator = parser.Compile(format);
            string result = generator.Render(true);
            Assert.AreEqual(String.Empty, result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the footer has content in front of it, the content should be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_IfNewLineContentEndIf_PrintsContent()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = @"{{#if this}}
Content{{/if}}";
            Generator generator = parser.Compile(format);
            string result = generator.Render(true);
            Assert.AreEqual("Content", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the header has content after it, the content should be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_IfContentNewLineEndIf_PrintsContent()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = @"{{#if this}}Content
{{/if}}";
            Generator generator = parser.Compile(format);
            string result = generator.Render(true);
            Assert.AreEqual("Content", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the header has content after it, the content should be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_ContentIfNewLineEndIf_PrintsContent()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = @"Content{{#if this}}
{{/if}}";
            Generator generator = parser.Compile(format);
            string result = generator.Render(true);
            Assert.AreEqual("Content", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the header and footer are adjacent, then there is no content.
        /// </summary>
        [TestMethod]
        public void TestCompile_IfEndIf_PrintsNothing()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = @"{{#if this}}{{/if}}";
            Generator generator = parser.Compile(format);
            string result = generator.Render(true);
            Assert.AreEqual(String.Empty, result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the header and footer are adjacent, then there is no inner content.
        /// </summary>
        [TestMethod]
        public void TestCompile_ContentIfEndIf_PrintsNothing()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = @"Content{{#if this}}{{/if}}";
            Generator generator = parser.Compile(format);
            string result = generator.Render(true);
            Assert.AreEqual("Content", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the header and footer are adjacent, then there is no inner content.
        /// </summary>
        [TestMethod]
        public void TestCompile_IfNewLineCommentEndIf_PrintsNothing()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = @"{{#if this}}
{{#! comment}}{{/if}}";
            Generator generator = parser.Compile(format);
            string result = generator.Render(true);
            Assert.AreEqual(String.Empty, result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the a header follows a footer, it shouldn't generate a new line.
        /// </summary>
        [TestMethod]
        public void TestCompile_IfNewLineContentNewLineEndIfIfNewLineContenNewLineEndIf_PrintsContent()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = @"{{#if this}}
First
{{/if}}{{#if this}}
Last
{{/if}}";
            Generator generator = parser.Compile(format);
            string result = generator.Render(true);
            const string expected = @"First
Last";
            Assert.AreEqual(expected, result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the content separates two if statements, it should be unaffected.
        /// </summary>
        [TestMethod]
        public void TestCompile_IfNewLineEndIfNewLineContentNewLineIfNewLineEndIf_PrintsContent()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = @"{{#if this}}
{{/if}}
Content
{{#if this}}
{{/if}}";
            Generator generator = parser.Compile(format);
            string result = generator.Render(true);
            const string expected = @"Content";
            Assert.AreEqual(expected, result, "The wrong text was generated.");
        }

        /// <summary>
        /// If there is trailing text of any kind, the newline after content should be preserved.
        /// </summary>
        [TestMethod]
        public void TestCompile_IfNewLineEndIfNewLineContentNewLineIfNewLineEndIfContent_PrintsContent()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = @"{{#if this}}
{{/if}}
First
{{#if this}}
{{/if}}
Last";
            Generator generator = parser.Compile(format);
            string result = generator.Render(true);
            const string expected = @"First
Last";
            Assert.AreEqual(expected, result, "The wrong text was generated.");
        }

        #endregion

        #region If/Else

        /// <summary>
        /// If the condition evaluates to false, the content of an else statement should be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_IfElse_EvaluatesToFalse_PrintsElse()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if this}}Yay{{#else}}Nay{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(false);
            Assert.AreEqual("BeforeNayAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the condition evaluates to true, the content of an if statement should be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_IfElse_EvaluatesToTrue_PrintsIf()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if this}}Yay{{#else}}Nay{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(true);
            Assert.AreEqual("BeforeYayAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// Second else blocks will result in an exceptions being thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestCompile_IfElse_TwoElses_IncludesSecondElseInElse_Throws()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if this}}Yay{{#else}}Nay{{#else}}Bad{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(false);
            Assert.AreEqual("BeforeNay{{#else}}BadAfter", result, "The wrong text was generated.");
        }

        #endregion

        #region If/Elif/Else

        /// <summary>
        /// If the if statement evaluates to true, its block should be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_IfElifElse_IfTrue_PrintsIf()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if First}}First{{#elif Second}}Second{{#else}}Third{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(new { First = true, Second = true });
            Assert.AreEqual("BeforeFirstAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the elif statement evaluates to true, its block should be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_IfElifElse_ElifTrue_PrintsIf()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if First}}First{{#elif Second}}Second{{#else}}Third{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(new { First = false, Second = true });
            Assert.AreEqual("BeforeSecondAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the elif statement evaluates to false, the else block should be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_IfElifElse_ElifFalse_PrintsElse()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if First}}First{{#elif Second}}Second{{#else}}Third{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(new { First = false, Second = false });
            Assert.AreEqual("BeforeThirdAfter", result, "The wrong text was generated.");
        }

        #endregion

        #region If/Elif

        /// <summary>
        /// If the elif statement evaluates to false and there is no else statement, nothing should be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_IfElif_ElifFalse_PrintsNothing()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if First}}First{{#elif Second}}Second{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(new { First = false, Second = false });
            Assert.AreEqual("BeforeAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If there are two elif statements and the first is false, the second elif block should be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_IfElifElif_ElifFalse_PrintsSecondElif()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if First}}First{{#elif Second}}Second{{#elif Third}}Third{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(new { First = false, Second = false, Third = true });
            Assert.AreEqual("BeforeThirdAfter", result, "The wrong text was generated.");
        }

        #endregion

        #region Each

        /// <summary>
        /// If we pass an empty collection to an each statement, the content should not be printed.
        /// </summary>
        [TestMethod]
        public void TestCompile_Each_EmptyCollection_SkipsContent()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#each this}}{{this}}{{/each}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(new int[0]);
            Assert.AreEqual("BeforeAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If we pass a populated collection to an each statement, the content should be printed
        /// for each item in the collection, using that item as the new scope context.
        /// </summary>
        [TestMethod]
        public void TestCompile_Each_PopulatedCollection_PrintsContentForEach()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#each this}}{{this}}{{/each}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(new int[] { 1, 2, 3 });
            Assert.AreEqual("Before123After", result, "The wrong text was generated.");
        }

        #endregion

        #region With

        /// <summary>
        /// The object replacing the placeholder should be used as the context of a with statement.
        /// </summary>
        [TestMethod]
        public void TestCompile_With_AddsScope()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#with Nested}}{{this}}{{/with}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(new { Nested = "Hello" });
            Assert.AreEqual("BeforeHelloAfter", result, "The wrong text was generated.");
        }

        #endregion

        #region Default Parameter

        /// <summary>
        /// If a tag is defined with a default parameter, the default value 
        /// should be returned if an argument is not provided.
        /// </summary>
        [TestMethod]
        public void TestCompile_MissingDefaultParameter_ProvidesDefault()
        {
            FormatCompiler compiler = new FormatCompiler();
            compiler.RegisterTag(new DefaultTagDefinition(), true);
            const string format = @"{{#default}}";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(null);
            Assert.AreEqual("123", result, "The wrong text was generated.");
        }

        private sealed class DefaultTagDefinition : InlineTagDefinition
        {
            public DefaultTagDefinition()
                : base("default")
            {
            }

            protected override bool GetIsContextSensitive()
            {
                return false;
            }

            protected override IEnumerable<TagParameter> GetParameters()
            {
                return new TagParameter[] { new TagParameter("param") { IsRequired = false, DefaultValue = 123 } };
            }

            protected override string GetText(IFormatProvider provider, Dictionary<string, object> arguments)
            {
                return arguments["param"].ToString();
            }
        }

        #endregion

        #region Compound Tags

        /// <summary>
        /// If a format contains multiple tags, they should be handled just fine.
        /// </summary>
        [TestMethod]
        public void TestCompile_MultipleTags()
        {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"Hello {{Customer.FirstName}}:

{{#with Order}}
{{#if LineItems}}
Below are your order details:

{{#each LineItems}}
    {{Name}}: {{UnitPrice:C}} x {{Quantity}}
{{/each}}

Your order total was: {{Total:C}}
{{/if}}
{{/with}}";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new
            {
                Customer = new { FirstName = "Bob" },
                Order = new
                {
                    Total = 7.50m,
                    LineItems = new object[] 
                    {
                        new { Name = "Banana", UnitPrice = 2.50m, Quantity = 1 },
                        new { Name = "Orange", UnitPrice = .50m, Quantity = 5 },
                        new { Name = "Apple", UnitPrice = .25m, Quantity = 10 },
                    }
                }
            });
            const string expected = @"Hello Bob:

Below are your order details:

    Banana: $2.50 x 1
    Orange: $0.50 x 5
    Apple: $0.25 x 10

Your order total was: $7.50";
            Assert.AreEqual(expected, result, "The wrong text was generated.");
        }

        #endregion
    }
}
