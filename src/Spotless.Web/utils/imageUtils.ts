export const getServiceImage = (serviceName: string): string => {
    const name = serviceName.toLowerCase();
    if (name.includes('shirt')) return 'https://images.unsplash.com/photo-1604654894610-df63bc536371?w=800&q=80';
    if (name.includes('suit')) return 'https://images.unsplash.com/photo-1594938298603-c8148c472997?w=800&q=80';
    if (name.includes('dress')) return 'https://images.unsplash.com/photo-1595777457583-95e059d581b8?w=800&q=80';
    if (name.includes('pants') || name.includes('trousers')) return 'https://images.unsplash.com/photo-1624378439575-d8705ad7ae80?w=800&q=80';
    if (name.includes('jacket') || name.includes('coat')) return 'https://images.unsplash.com/photo-1591047139829-d91aecb6caea?w=800&q=80';
    if (name.includes('bed') || name.includes('sheet') || name.includes('duvet')) return 'https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=800&q=80';
    if (name.includes('curtain')) return 'https://images.unsplash.com/photo-1513694203232-719a280e022f?w=800&q=80';
    if (name.includes('shoe') || name.includes('sneaker')) return 'https://images.unsplash.com/photo-1560769629-975e1216f110?w=800&q=80';
    if (name.includes('carpet') || name.includes('rug')) return 'https://images.unsplash.com/photo-1575052814086-f385e2e2ad1b?w=800&q=80';
    if (name.includes('dry') || name.includes('clean')) return 'https://images.unsplash.com/photo-1582735689369-4fe89db7114c?w=800&q=80';
    if (name.includes('wash') || name.includes('fold')) return 'https://images.unsplash.com/photo-1517677208171-0bc12dd9aaea?w=800&q=80';
    if (name.includes('iron')) return 'https://images.unsplash.com/photo-1489278530698-d4757abdff0f?w=800&q=80';
    if (name.includes('leather')) return 'https://images.unsplash.com/photo-1559563458-52c695252d71?w=800&q=80';
    if (name.includes('wedding')) return 'https://images.unsplash.com/photo-1596451190630-186aff535bf2?w=800&q=80';
    return 'https://images.unsplash.com/photo-1545173168-9f1947eebb8f?w=800&q=80'; // Default laundry image
};

export const getCategoryImage = (categoryName: string): string => {
    const name = categoryName.toLowerCase();
    if (name.includes('men')) return 'https://images.unsplash.com/photo-1617137984095-74e4e5e3613f?w=800&q=80';
    if (name.includes('women')) return 'https://images.unsplash.com/photo-1483985988355-763728e1935b?w=800&q=80';
    if (name.includes('kid') || name.includes('child')) return 'https://images.unsplash.com/photo-1622290291468-a28f7a7dc6a8?w=800&q=80';
    if (name.includes('home') || name.includes('house')) return 'https://images.unsplash.com/photo-1556909212-d5b604d0c90d?w=800&q=80';
    if (name.includes('shoe')) return 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=800&q=80';
    if (name.includes('premium') || name.includes('delicate')) return 'https://images.unsplash.com/photo-1490481651871-ab68de25d43d?w=800&q=80';
    return 'https://images.unsplash.com/photo-1582735689369-4fe89db7114c?w=800&q=80'; // Default
};
