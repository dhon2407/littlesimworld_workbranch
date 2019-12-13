using UnityEngine;

namespace InventorySystem
{
    public class ItemBuilder
    {
        public Item Build(ItemCode itemCode)
        {
            return Resources.Load<GameObject>(ItemNames.Get(itemCode)).GetComponent<Item>();
        }

        private static class ItemNames
        {
            private static readonly string Resourcelocation = "Prefabs/Items/";

            public static string Get(ItemCode code)
            {
                switch (code)
                {
                    //Add new items here
                    case ItemCode.BREAD:                    return Resourcelocation + "Food/Ingredients/Bread";
                    case ItemCode.CHICKEN_BREAST:           return Resourcelocation + "Food/Chicken breast";
                    case ItemCode.COKE_DIET:                return Resourcelocation + "Food/Cork Diet";
                    case ItemCode.COKE:                     return Resourcelocation + "Food/Cork";
                    case ItemCode.COOKED_EGG:               return Resourcelocation + "Food/Cooked Egg";
                    case ItemCode.COOKED_FISH:              return Resourcelocation + "Food/Cooked Fish";
                    case ItemCode.COOKED_SAUSAGE:           return Resourcelocation + "Food/Cooked Sausages";
                    case ItemCode.COOKED_TOAST:             return Resourcelocation + "Food/Cooked Toast";
                    case ItemCode.CROISSANT:                return Resourcelocation + "Food/Croissant";
                    case ItemCode.EGG_ON_TOAST:             return Resourcelocation + "Food/Egg on Toast";
                    case ItemCode.EGG_SALAD:                return Resourcelocation + "Food/Egg Salad";
                    case ItemCode.EGG:                      return Resourcelocation + "Food/Ingredients/Eggs";
                    case ItemCode.ENGLISH_BREAKFAST:        return Resourcelocation + "Food/English Breakfast";
                    case ItemCode.FISH_AND_CHIPS:           return Resourcelocation + "Food/Fish and Chips";
                    case ItemCode.FISH_OMELETTE:            return Resourcelocation + "Food/Fish Omelette";
                    case ItemCode.FISH:                     return Resourcelocation + "Food/Ingredients/Fish";
                    case ItemCode.FISHFINGERS:              return Resourcelocation + "Food/Fishfingers";
                    case ItemCode.FRIED_EGG:                return Resourcelocation + "Food/Fried Egg";
                    case ItemCode.GOURMENT_SANDWICH:        return Resourcelocation + "Food/Gourmet Sandwich";
                    case ItemCode.JELLY:                    return Resourcelocation + "Food/Jelly";
                    case ItemCode.MEAT_STEW:                return Resourcelocation + "Food/Meat Stew";
                    case ItemCode.MEAT:                     return Resourcelocation + "Food/Ingredients/Meat";
                    case ItemCode.OMELLETE_HAM:             return Resourcelocation + "Food/Omelette with Ham";
                    case ItemCode.OMELLETE:                 return Resourcelocation + "Food/Omelette";
                    case ItemCode.ROAST_DINNER:             return Resourcelocation + "Food/Roast Dinner";
                    case ItemCode.SALAD_SUPRERME:           return Resourcelocation + "Food/Salad Supreme";
                    case ItemCode.SALAD:                    return Resourcelocation + "Food/Salad";
                    case ItemCode.SAUGAGES:                 return Resourcelocation + "Food/Sausages";
                    case ItemCode.SIDE_SALAD:               return Resourcelocation + "Food/Side Salad";
                    case ItemCode.VEGETABLE:                return Resourcelocation + "Food/Ingredients/Vegetable";
                    case ItemCode.VEGGY_SANDWICH:           return Resourcelocation + "Food/Veggy Sandwich";
                    case ItemCode.BURGER:                   return Resourcelocation + "Food/Burger";
                    case ItemCode.WATER:                    return Resourcelocation + "Food/Water";
                    case ItemCode.BEER:                    return Resourcelocation + "Food/Beer";
                    case ItemCode.BED2:                    return Resourcelocation + "Furnitures/Bed2";
                    case ItemCode.BED3:                    return Resourcelocation + "Furnitures/Bed3";
                    case ItemCode.STOVE2:                    return Resourcelocation + "Furnitures/Stove2";
                    case ItemCode.STOVE3:                    return Resourcelocation + "Furnitures/Stove3";
                    case ItemCode.DESK2:                    return Resourcelocation + "Furnitures/Desk2";
                    case ItemCode.DESK3:                    return Resourcelocation + "Furnitures/Desk3";
                    case ItemCode.SHOWER2:                    return Resourcelocation + "Furnitures/Shower2";
                    case ItemCode.SHOWER3:                    return Resourcelocation + "Furnitures/Shower3";
                    case ItemCode.TOILET2:                    return Resourcelocation + "Furnitures/Toilet2";
                    case ItemCode.TOILET3:                    return Resourcelocation + "Furnitures/Toilet3";

                    default:
                        throw new UnityException("Unknow itemCode, unable to build item.");
                }
            }
        }

    }
}
