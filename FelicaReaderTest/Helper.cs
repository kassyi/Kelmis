using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelicaReaderTest
{
    /// <summary>
    /// 独自 Assert クラス
    /// </summary>
    public static class AssertEx
    {
        /// <summary>
        /// 指定の例外が発生することを検証する。
        /// </summary>
        /// <typeparam name="T">例外型</typeparam>
        /// <param name="action">実行処理</param>
        /// <returns>発生した例外インスタンス</returns>
        /// <remarks>MSTest には存在しないので、NUnit から一部だけ輸入</remarks>
        public static T Throws<T>(Action action) where T : Exception
        {
            T caught = null;
            try
            {
                action();
                Assert.Fail("例外が発生しませんでした。");
            }
            catch (T ex)
            {
                caught = ex;
            }
            Assert.AreEqual(caught.GetType(), typeof(T));
            return caught;
        }
    }
}
