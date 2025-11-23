export const getServiceImage = (serviceName: string): string => {
    const name = serviceName.toLowerCase();

    // Suits & Formal
    if (name.includes('tuxedo')) return 'https://images.unsplash.com/photo-1593032465175-481ac7f401a0?w=800&q=80';
    if (name.includes('suit') || name.includes('blazer')) return 'https://images.unsplash.com/photo-1594938298603-c8148c472997?w=800&q=80';
    if (name.includes('vest') || name.includes('waistcoat')) return 'https://images.unsplash.com/photo-1617127365659-c47fa864d8bc?w=800&q=80';
    if (name.includes('tie') || name.includes('cravat')) return 'https://images.unsplash.com/photo-1589756823695-278bc35616b5?w=800&q=80';

    // Dresses & Gowns
    if (name.includes('wedding')) return 'https://images.unsplash.com/photo-1596451190630-186aff535bf2?w=800&q=80';
    if (name.includes('evening') || name.includes('gown')) return 'https://images.unsplash.com/photo-1566174053879-31528523f8ae?w=800&q=80';
    if (name.includes('cocktail')) return 'https://images.unsplash.com/photo-1595777457583-95e059d581b8?w=800&q=80';
    if (name.includes('dress')) return 'https://images.unsplash.com/photo-1539008835657-9e8e9680c956?w=800&q=80';
    if (name.includes('skirt')) return 'https://images.unsplash.com/photo-1583496661160-fb5886a0aaaa?w=800&q=80';

    // Tops
    if (name.includes('blouse')) return 'https://images.unsplash.com/photo-1564257631407-4deb1f99d992?w=800&q=80';
    if (name.includes('shirt')) return 'https://images.unsplash.com/photo-1604654894610-df63bc536371?w=800&q=80';
    if (name.includes('t-shirt') || name.includes('polo')) return 'https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=800&q=80';
    if (name.includes('sweater') || name.includes('pullover') || name.includes('cardigan')) return 'https://images.unsplash.com/photo-1620799140408-ed5341cd2431?w=800&q=80';
    if (name.includes('hoodie') || name.includes('sweatshirt')) return 'https://images.unsplash.com/photo-1556905055-8f358a7a47b2?w=800&q=80';

    // Bottoms
    if (name.includes('jeans') || name.includes('denim')) return 'https://images.unsplash.com/photo-1542272617-08f086302542?w=800&q=80';
    if (name.includes('shorts')) return 'https://images.unsplash.com/photo-1591195853828-11db59a44f6b?w=800&q=80';
    if (name.includes('pants') || name.includes('trousers')) return 'https://images.unsplash.com/photo-1624378439575-d8705ad7ae80?w=800&q=80';

    // Outerwear
    if (name.includes('coat') || name.includes('overcoat') || name.includes('trench')) return 'https://images.unsplash.com/photo-1539533018447-63fcce2678e3?w=800&q=80';
    if (name.includes('jacket') || name.includes('windbreaker')) return 'https://images.unsplash.com/photo-1591047139829-d91aecb6caea?w=800&q=80';
    if (name.includes('leather')) return 'https://images.unsplash.com/photo-1559563458-52c695252d71?w=800&q=80';
    if (name.includes('fur')) return 'https://images.unsplash.com/photo-1559563458-52c695252d71?w=800&q=80'; // Reusing leather/fur vibe

    // Household & Bedding
    if (name.includes('duvet') || name.includes('comforter')) return 'https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=800&q=80';
    if (name.includes('pillow')) return 'https://images.unsplash.com/photo-1584100936595-c0654b55a2e6?w=800&q=80';
    if (name.includes('sheet') || name.includes('linen')) return 'https://images.unsplash.com/photo-1522771753035-4a50354b6b00?w=800&q=80';
    if (name.includes('blanket') || name.includes('throw')) return 'https://images.unsplash.com/photo-1580480055273-228ff5388ef8?w=800&q=80';
    if (name.includes('curtain') || name.includes('drape')) return 'https://images.unsplash.com/photo-1513694203232-719a280e022f?w=800&q=80';
    if (name.includes('towel')) return 'https://images.unsplash.com/photo-1616627547584-bf28ceeecdb4?w=800&q=80';
    if (name.includes('tablecloth') || name.includes('napkin')) return 'https://images.unsplash.com/photo-1574625865239-1c9c3482d80c?w=800&q=80';
    if (name.includes('carpet') || name.includes('rug') || name.includes('mat')) return 'https://images.unsplash.com/photo-1575052814086-f385e2e2ad1b?w=800&q=80';

    // Shoes
    if (name.includes('sneaker') || name.includes('trainer')) return 'https://images.unsplash.com/photo-1560769629-975e1216f110?w=800&q=80';
    if (name.includes('boot')) return 'https://images.unsplash.com/photo-1608256246200-53e635b5b65f?w=800&q=80';
    if (name.includes('heel') || name.includes('pump')) return 'https://images.unsplash.com/photo-1543163521-1bf539c55dd2?w=800&q=80';
    if (name.includes('shoe') || name.includes('loafer')) return 'https://images.unsplash.com/photo-1549298916-b41d501d3772?w=800&q=80';

    // Accessories
    if (name.includes('bag') || name.includes('purse') || name.includes('handbag')) return 'https://images.unsplash.com/photo-1584917865442-de89df76afd3?w=800&q=80';
    if (name.includes('scarf') || name.includes('shawl')) return 'https://images.unsplash.com/photo-1584030373081-f37b7bb4fa8e?w=800&q=80';
    if (name.includes('hat') || name.includes('cap')) return 'https://images.unsplash.com/photo-1588850561407-ed78c282e89f?w=800&q=80';

    // General
    if (name.includes('dry') || name.includes('clean')) return 'https://images.unsplash.com/photo-1582735689369-4fe89db7114c?w=800&q=80';
    if (name.includes('wash') || name.includes('fold') || name.includes('laundry')) return 'https://images.unsplash.com/photo-1517677208171-0bc12dd9aaea?w=800&q=80';
    if (name.includes('iron') || name.includes('press')) return 'https://images.unsplash.com/photo-1489278530698-d4757abdff0f?w=800&q=80';

    return 'https://images.unsplash.com/photo-1545173168-9f1947eebb8f?w=800&q=80'; // Default laundry image
};

export const getCategoryImage = (categoryName: string): string => {
    const name = categoryName.toLowerCase();
    if (name.includes('men')) return 'https://images.unsplash.com/photo-1617137984095-74e4e5e3613f?w=800&q=80';
    if (name.includes('women')) return 'https://images.unsplash.com/photo-1483985988355-763728e1935b?w=800&q=80';
    if (name.includes('kid') || name.includes('child') || name.includes('baby')) return 'https://images.unsplash.com/photo-1622290291468-a28f7a7dc6a8?w=800&q=80';
    if (name.includes('home') || name.includes('house') || name.includes('household')) return 'https://images.unsplash.com/photo-1556909212-d5b604d0c90d?w=800&q=80';
    if (name.includes('shoe') || name.includes('footwear')) return 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=800&q=80';
    if (name.includes('premium') || name.includes('delicate') || name.includes('luxury')) return 'https://images.unsplash.com/photo-1490481651871-ab68de25d43d?w=800&q=80';
    if (name.includes('bed') || name.includes('bath')) return 'https://images.unsplash.com/photo-1584622650111-993a426fbf0a?w=800&q=80';
    return 'https://images.unsplash.com/photo-1582735689369-4fe89db7114c?w=800&q=80'; // Default
};
