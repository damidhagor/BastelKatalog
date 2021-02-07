using System;
using System.Collections.Generic;
using System.Text;

namespace BastelKatalog.Models
{
    /// <summary>
    /// Extensions for the Items and ItemWrappers
    /// </summary>
    public static class ItemWrapperExtensions
    {
        /// <summary>
        /// Creates an ItemWrapper from an Item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static ItemWrapper ToItemWrapper(this Data.Item item)
        {
            ItemWrapper wrapper = new ItemWrapper(item);
            return wrapper;
        }
    }
}
