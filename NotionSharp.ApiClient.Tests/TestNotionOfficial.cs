﻿using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NotionSharp.ApiClient.Tests.Lib;

namespace NotionSharp.ApiClient.Tests
{
    [TestClass]
    public class TestNotionOfficial
    {
        /// <summary>
        /// Automatic paging
        /// </summary>
        [TestMethod]
        public async Task TestSearch()
        {
            var session = new NotionSession(TestUtils.CreateOfficialNotionSessionInfo());

            var pagingOptions = new PagingOptions {PageSize = 2};
            var totalItems = 0;
            await foreach (var item in session.Search(pagingOptions: pagingOptions))
            {
                Assert.IsTrue(item is JsonElement);
                totalItems++;
            }

            Assert.AreEqual(4, totalItems);
        }

        /// <summary>
        /// Manual paging
        /// </summary>
        [TestMethod]
        public async Task TestSearchPaged()
        {
            var session = new NotionSession(TestUtils.CreateOfficialNotionSessionInfo());

            var pagingOptions = new PagingOptions {PageSize = 2};
            var searchResult = await session.SearchPaged(pagingOptions: pagingOptions);
            Assert.IsNotNull(searchResult?.Results);
            Assert.IsTrue(searchResult.Results[0] is JsonElement);

            var totalItems = searchResult.Results.Count;
            var i = 10;
            while (searchResult.HasMore && i-- > 0)
            {
                pagingOptions.StartCursor = searchResult.NextCursor;
                searchResult = await session.SearchPaged(pagingOptions: pagingOptions);
                totalItems += searchResult.Results.Count;
            }
            Assert.AreNotEqual(0, i);
            Assert.AreEqual(4, totalItems);
        }
        
        // [TestMethod]
        // public async Task TestLoadPageChunkResult()
        // {
        //     var session = new NotionSession(TestUtils.CreateOfficialNotionSessionInfo());
        //
        //     var pageChunk = await session.LoadPageChunk(Guid.Parse("4e4999b4-161a-449d-bbd1-bdbce690c7cb"), 0, 50);
        //     Assert.IsNotNull(pageChunk);
        //     Assert.IsTrue(pageChunk.RecordMap.Block.Count > 0);
        // }
    }
}
