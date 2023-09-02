using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestJob
{

    public interface IHasMeta
    {
        MetaData GetMeta();
    }

    [Serializable]
    public class MetaData
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
    }

}
