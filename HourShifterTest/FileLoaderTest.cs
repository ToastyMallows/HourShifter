using HourShifter;
using NUnit.Framework;
using System;
using System.Collections;
using System.IO;

namespace HourShifterTest
{
    [TestFixture]
    public class FileLoaderTest
    {
        private static IEnumerable FileLoader_InvalidCurrentDirectories
        {
            get
            {
                yield return new TestCaseData( null );
                yield return new TestCaseData( string.Empty );
                yield return new TestCaseData( " " );
                yield return new TestCaseData( string.Empty + Environment.NewLine );
                yield return new TestCaseData( '\t' + " " );
            }
        }

        [Test]
        [Description("Throws for invalid current directories")]
        [TestCaseSource( "FileLoader_InvalidCurrentDirectories" )]
        public void FileLoader_Throws_NullOrWhitespaceCurrentDirectory(string currentDirectory)
        {
            Assert.That( () => { new FileLoader( currentDirectory, false ); }, Throws.ArgumentException );
        }

        [Test]
        [Description("AllPaths shouldn't return null when running in the current test directory")]
        public void FileLoader_AllPathsNotNull()
        {
            FileLoader fileLoader = new FileLoader( Directory.GetCurrentDirectory(), false );
            Assert.That( fileLoader.AllPaths, Is.Not.Null.Or.Empty );
        }

        // TODO: Tests for the currentDirectoryOnly boolean
    }
}
