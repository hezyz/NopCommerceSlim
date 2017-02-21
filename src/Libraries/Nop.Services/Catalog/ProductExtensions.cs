using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class ProductExtensions
    {        
        /// <summary>
        /// Finds a related product item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="productId2">The second product identifier</param>
        /// <returns>Related product</returns>
        public static RelatedProduct FindRelatedProduct(this IList<RelatedProduct> source,
            int productId1, int productId2)
        {
            foreach (RelatedProduct relatedProduct in source)
                if (relatedProduct.ProductId1 == productId1 && relatedProduct.ProductId2 == productId2)
                    return relatedProduct;
            return null;
        }

        /// <summary>
        /// Indicates whether a product tag exists
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>Result</returns>
        public static bool ProductTagExists(this Product product,
            int productTagId)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            bool result = product.ProductTags.ToList().Find(pt => pt.Id == productTagId) != null;
            return result;
        }


        /// <summary>
        /// Gets SKU
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="sku">SKU</param>
        private static void GetSkuMpnGtin(this Product product,
            out string sku)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            sku = null;

            if (String.IsNullOrEmpty(sku))
                sku = product.Sku;
        }

        /// <summary>
        /// Formats SKU
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>SKU</returns>
        public static string FormatSku(this Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            string sku;

            product.GetSkuMpnGtin(
                out sku);

            return sku;
        }

    }
}
