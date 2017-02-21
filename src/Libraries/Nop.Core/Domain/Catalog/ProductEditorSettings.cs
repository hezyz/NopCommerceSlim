
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Catalog
{
    public class ProductEditorSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether 'ID' field is shown
        /// </summary>
        public bool Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Product type' field is shown
        /// </summary>
        public bool ProductType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Visible individually' field is shown
        /// </summary>
        public bool VisibleIndividually { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Product template' field is shown
        /// </summary>
        public bool ProductTemplate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Admin comment' feild is shown
        /// </summary>
        public bool AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Stores' field is shown
        /// </summary>
        public bool Stores { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'ACL' field is shown
        /// </summary>
        public bool ACL { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Show on home page' field is shown
        /// </summary>
        public bool ShowOnHomePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Display order 'field is shown
        /// </summary>
        public bool DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Allow customer reviews' field is shown
        /// </summary>
        public bool AllowCustomerReviews { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Product tags' field is shown
        /// </summary>
        public bool ProductTags { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Mark as new' field is shown
        /// </summary>
        public bool MarkAsNew { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Mark as new. Start date' field is shown
        /// </summary>
        public bool MarkAsNewStartDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Mark as new. End date' field is shown
        /// </summary>
        public bool MarkAsNewEndDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Published' field is shown
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Created on' field is shown
        /// </summary>
        public bool CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Updated on' field is shown
        /// </summary>
        public bool UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Related products' block is shown
        /// </summary>
        public bool RelatedProducts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'SEO' tab is shown
        /// </summary>
        public bool Seo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether one column is used on the product details page
        /// </summary>
        public bool OneColumnProductPage { get; set; }

    }
}