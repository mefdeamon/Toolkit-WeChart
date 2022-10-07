namespace WindChart
{
    /// <summary>
    /// 轴刻度线位置
    /// </summary>
    public enum AxisLineMode
    {
        /// <summary>
        /// 网格线
        /// </summary>
        Grid,
        /// <summary>
        /// 顶部/左边
        /// </summary>
        TopLeft,
        /// <summary>
        /// 底部/右边
        /// </summary>
        BottmRight,
        /// <summary>
        /// 根据定位来
        /// </summary>
        Location,
        /// <summary>
        /// 中心
        /// </summary>
        Center,
    }

    /// <summary>
    /// 轴刻度文本位置
    /// </summary>
    public enum AxisTextMode
    {
        /// <summary>
        /// 两边都有
        /// </summary>
        Both,
        /// <summary>
        /// 顶部/左边
        /// </summary>
        TopLeft,
        /// <summary>
        /// 底部/右边
        /// </summary>
        BottomRight
    }
}
