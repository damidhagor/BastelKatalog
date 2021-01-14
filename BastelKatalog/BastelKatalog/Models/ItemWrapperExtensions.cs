using System;
using System.Collections.Generic;
using System.Text;

namespace BastelKatalog.Models
{
    public static class ItemWrapperExtensions
    {
        public static ItemWrapper ToItemWrapper(this Data.Item item)
        {
            ItemWrapper wrapper = new ItemWrapper(item);
            return wrapper;
        }
    }
}
