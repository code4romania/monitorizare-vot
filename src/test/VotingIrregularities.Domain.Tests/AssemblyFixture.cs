﻿using System;

namespace VotingIrregularities.Domain.Tests
{
    public class AssemblyFixture : IDisposable
    {
        public static AssemblyFixture Current = new AssemblyFixture();

        private AssemblyFixture()
        {
        }

        ~AssemblyFixture()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
