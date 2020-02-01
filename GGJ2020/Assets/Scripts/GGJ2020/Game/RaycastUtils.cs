using UnityEngine;

namespace GGJ2020.Game
{
    public class RaycastUtils
    {
        public static bool RaycastMouse(out RaycastHit outHit, LayerMask layerMask)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, layerMask))
            {
                outHit = hit;
                return true;
            }
            
            outHit = new RaycastHit();
            return false;
        }
        
        public static bool RaycastMouse(out RaycastHit outHit)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                outHit = hit;
                return true;
            }
            
            outHit = new RaycastHit();
            return false;
        }
    }
}