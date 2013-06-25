using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elphysics
{
    class BusyInCollision : DBase<BusyInCollision>
    {
        private Guid CBallId;
        public CBall CBall
        {
            get { return CBall.GetItemById(CBallId); }
            set { CBallId = value.GUID; }
        }

        private Guid CCollisionId;
        public CCollision CCollision
        {
            get { return CCollision.GetItemById(CCollisionId); }
            set { CCollisionId = value.GUID; }
        }
    }
}
