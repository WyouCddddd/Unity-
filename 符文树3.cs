using UnityEngine;
using static CMKZ.LocalStorage;//永恒库
using CMKZ;
using UnityEngine.UI;
using static UnityEngine.Object;
using static 符文树3.LocalStorage;
using TMPro;
using Unity.VisualScripting;
using System;
//符文树库
namespace 符文树3 {
    public static partial class LocalStorage {
        public static Dictionary<string, Action> DiUpdateDo {
            get {
                if (!GameObjectPool.ContainsKey("Update2")) {
                    GameObjectPool.Add("Update2", new GameObject("Update", typeof(MainThread1)));
                }
                return GameObjectPool["Update2"].GetComponent<MainThread1>().命名任务的队列;
            }
        }
        public class MainThread1 : MonoBehaviour {
            public Dictionary<string, Action> 命名任务的队列 = new Dictionary<string, Action>();
            private void Update() {
                foreach (var ins in 命名任务的队列) ins.Value();
            }
        }
        /// <summary>
        /// 获得两向量间的夹角，初向量（fromVector）转到末向量（toVector），逆时针为正，顺时针为负
        /// </summary>
        public static float 获得两向量间的夹角(Vector3 fromVector, Vector3 toVector) {
            float angle = Vector3.Angle(fromVector, toVector); //求出两向量之间的夹角
            Vector3 normal = Vector3.Cross(fromVector, toVector);//叉乘求出法线向量
            angle *= Mathf.Sign(Vector3.Dot(normal, new Vector3(0, 0, 1)));
            return angle;
        }
        public static void 允许拖动缩放(GameObject X) {
            if (!游戏物体池.ContainsKey("Arrow")) {
                GameObject A = new GameObject();
                A.transform.SetParent(MainPanel.transform);
                A.AddComponent<Image>();
                A.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 30);
                A.GetComponent<Image>().sprite = AllSprite["Arrow"];
                游戏物体池.Add("Arrow", A);
            }
            X.AddComponent<Window>().icon = 游戏物体池["Arrow"].GetComponent<RectTransform>();
        }
        public static GameObject 两点间连线(Vector3 起始点, Vector3 末点) {
            GameObject A = new GameObject();
            A.transform.SetParent(MainPanel.transform);
            A.AddComponent<Image>();
            A.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
            float Angle = 获得两向量间的夹角(new Vector3(1, 0, 0), 末点 - 起始点);
            A.transform.position = 起始点;
            A.GetComponent<RectTransform>().localRotation = Quaternion.Euler(Vector3.forward * Angle);
            A.GetComponent<RectTransform>().sizeDelta = new Vector2((末点 - 起始点).magnitude, 20);
            A.GetComponent<Image>().sprite = AllSprite["Arrow"];
            return A;
        }
        public static void 线末端跟随鼠标(GameObject X) {
            Vector3 Ve = Input.mousePosition;
            Ve.z = 0;
            float Angle = 获得两向量间的夹角(new(1, 0, 0), Ve - X.transform.position);
            X.GetComponent<RectTransform>().localRotation = Quaternion.Euler(Vector3.forward * Angle);
            X.GetComponent<RectTransform>().sizeDelta = new Vector2((X.transform.position - Ve).magnitude, 20);
        }
        public static void 调整连线位置(GameObject X, Vector3 起始点, Vector3 末点) {
            float Angle = 获得两向量间的夹角(new Vector3(1, 0, 0), 末点 - 起始点);
            X.transform.position = 起始点;
            X.GetComponent<RectTransform>().localRotation = Quaternion.Euler(Vector3.forward * Angle);
            X.GetComponent<RectTransform>().sizeDelta = new Vector2((末点 - 起始点).magnitude, 20);
        }
        public static void Clear(this GameObject X) {
            Transform[] trans = X.GetComponentsInChildren<Transform>();
            foreach (var ins in trans) {
                if (ins != X.GetComponent<RectTransform>()) {
                    Destroy(ins.gameObject);
                }
            }
            //清空所有子物体
            //必须使用立刻删除，而不是延迟删除
        }
        public static void 仅显示此符文树(string ID) {          
            foreach (var ins in 存档.已创建的符文树) {
                foreach (var i in ins.Value.符文树节点) {
                    if (游戏物体池.ContainsKey(i.ID)) {
                        Destroy(游戏物体池[i.ID]);
                        游戏物体池.Remove(i.ID);
                    }
                }
                foreach (var i in ins.Value.符文树连线) {
                    if (游戏物体池.ContainsKey(i.ID)) {
                        Destroy(游戏物体池[i.ID]);
                        游戏物体池.Remove(i.ID);
                    }
                }              
            }
            if (存档.已创建的符文树[ID].数据类别 != "笔记本") {
                if (存档.已创建的符文树.Count != 0) {
                    foreach (var ins in 存档.已创建的符文树[ID].符文树节点) {
                        节点 JD = new 节点(ins.ID);
                    }
                    foreach (var ins in 存档.已创建的符文树[ID].符文树连线) {
                        连线 LX = new 连线(ins.ID);
                    }
                } 
            }
            else  {
                笔记本 BJ = new 笔记本(存档.已创建的符文树[ID].符文树节点[0].ID);
                游戏物体池[BJ.ID].transform.GetChild(0).GetComponent<TMP_InputField>().text = 存档.已创建的符文树[ID].符文树节点[0].内容;
                游戏物体池[BJ.ID].transform.GetChild(0).GetComponent<TMP_InputField>().onValueChanged.AddListener(t => 存档.已创建的符文树[ID].符文树节点[0].内容 = 游戏物体池[BJ.ID].transform.GetChild(0).GetComponent<TMP_InputField>().text);
            }
        }
        public static void 创建滚动列表(GameObject 父物体,string Y) {
            GameObject A = 父物体.创建矩形(Y);
            游戏物体池.Add("符文树列表", A.创建垂直布局滚动条());
        }
        public static void 连线位置基于坐标调整(string ID) {
            调整连线位置(游戏物体池[ID], 存档.已创建的符文树[存档.当前符文树].查找线条(ID).连线起点, 存档.已创建的符文树[存档.当前符文树].查找线条(ID).连线终点);
        }
        public static void 连线位置基于节点调整(string ID) {
            调整连线位置(游戏物体池[ID], 游戏物体池[存档.已创建的符文树[存档.当前符文树].查找线条(ID).初节点.ID].transform.position, 游戏物体池[存档.已创建的符文树[存档.当前符文树].查找线条(ID).末节点.ID].transform.position);
        }
        public static bool 判断鼠标是否在某UI上(RectTransform UI) {
            if (RectTransformUtility.RectangleContainsScreenPoint(UI, Input.mousePosition)) {
                return true;
            }
            return false;
        }
    }
}

//符文树数据
namespace 符文树3 {
    public static partial class LocalStorage {
        public static Dictionary<string, GameObject> 游戏物体池 = new Dictionary<string, GameObject>();
        public static 存档类 存档;
        public static 连线ing 正在连线 = new 连线ing();
        public static 符文树目录 符文树编辑器;
        public static void 执行存档() {
            FileWrite("C:/存档/符文树编辑器", 存档);
        }
        public static void 读档() {
           存档 = FileRead<存档类>("C:/存档/符文树编辑器") ?? new 存档类();
        }
    }
    public class 连线ing{
        public bool 当前正在连线 = false;
        public string 当前连线的节点;
        public string 当前连线的节点的判断条件;
        public 连线数据 不定连线;
    }
    public class 节点数据 {
        public string ID;
        public string 父节点;
        public List<string> 子节点 = new();
        public float 点击是的次数;
        public float 点击否的次数;
        public string 所属符文树;
        public string 内容;
        public Vector3 节点坐标 = new Vector3(1110,490,0);
    }
    public class 连线数据 {
        public string ID;
        public 节点数据 初节点;
        public 节点数据 末节点;
        public string 所属符文树;
        public Vector3 连线起点;
        public Vector3 连线终点;
    }
    public class 符文树数据 {
        public string 数据类别="符文树";//笔记本或者符文树
        public string ID;//符文树+ID
        public List<节点数据> 符文树节点 = new List<节点数据>();
        public List<连线数据> 符文树连线 = new List<连线数据>();
        public 节点数据 查找节点(string ID) {
            return 符文树节点.Find(t => t.ID == ID);
        }
        public 连线数据 查找线条(string ID) {
            return 符文树连线.Find(t => t.ID == ID);
        }
    }
    public class 笔记本数据 : 节点数据 {

    }
    public class 存档类 {
        public float 符文树条目ID;
        public float 符文树节点ID;
        public float 符文树连线ID;
        public float 笔记本ID;
        public Dictionary<string, 符文树数据> 已创建的符文树 = new Dictionary<string, 符文树数据>();
        public string 当前符文树 = null;
    }
}
//符文树UI
namespace 符文树3 {
    public class 符文树目录 {
        public 符文树目录() {
            GameObject A = MainPanel.创建矩形("0 0 30% 80%");
            A.AddComponent<Image>().color = new Color(195 / 255f, 134 / 255f, 134 / 255f, 255 / 255f);
            创建滚动列表(A, "15% 10% 85% 85%");
            刷新();
            仅显示此符文树(存档.当前符文树);
        }
        public void 刷新() {
            foreach (var ins2 in 存档.已创建的符文树) {
                Destroy(游戏物体池[ins2.Key]);
                游戏物体池.Remove(ins2.Key);
            }
            foreach (var ins in 存档.已创建的符文树) {
                GameObject A = 游戏物体池["符文树列表"].创建矩形("0 0 100 80");
                A.GetComponent<RectTransform>().pivot = new Vector2(0.1f, 0.9f);
                A.SetText(ins.Key);
                A.AddComponent<Image>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f); ;
                A.OnEnter(() => {
                    A.GetComponent<Image>().color = new Color(219 / 255f, 126 / 255f, 126 / 255f, 255 / 255f);
                });
                A.OnExit(() => {
                    A.GetComponent<Image>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);
                    游戏物体池[存档.当前符文树].GetComponent<Image>().color = new Color(246 / 255f, 58 / 255f, 58 / 255f, 255 / 255f);
                });
                游戏物体池.Add(ins.Key, A);
                A.OnClick(() => {
                    存档.当前符文树 = ins.Key;
                    仅显示此符文树(ins.Key);
                    颜色刷新();
                });             
            }
            颜色刷新();
        }
        public void 颜色刷新() {
            foreach (var ins in 存档.已创建的符文树) {
                游戏物体池[ins.Key].GetComponent<Image>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f); ;
            }
                if (存档.当前符文树 != null) {
                游戏物体池[存档.当前符文树].GetComponent<Image>().color = new Color(246 / 255f, 58 / 255f, 58 / 255f, 255 / 255f);
            }
        }
    }
    public class 符文树 {
        public void 仅显示此符文树() {
            foreach (var ins in 存档.已创建的符文树) {
                foreach (var i in ins.Value.符文树节点) {
                    if (游戏物体池.ContainsKey(i.ID)) {
                        Destroy(游戏物体池[i.ID]);
                        游戏物体池.Remove(i.ID);
                    }
                }
                foreach (var i in ins.Value.符文树连线) {
                    if (游戏物体池.ContainsKey(i.ID)) {
                        Destroy(游戏物体池[i.ID]);
                        游戏物体池.Remove(i.ID);
                    }
                }
            }

        }
    }
    public class 节点 {
        public string ID;
        private bool 是否正在连线 = false;
        private 符文树数据 当前符文树;
        public 节点(string ID1) {
            ID = ID1;
            当前符文树 = 存档.已创建的符文树[存档.当前符文树];
            GameObject A = MainPanel.创建矩形("50% 50% 300 100");//这是节点背景
            A.transform.position = 当前符文树.查找节点(ID).节点坐标;
            A.AddComponent<Image>();
            GameObject B = A.CreateInput("0 0 100% 60%");
            B.GetComponent<TMP_InputField>().onValueChanged.AddListener(t => 当前符文树.查找节点(ID).内容 = B.GetComponent<TMP_InputField>().text);
            B.GetComponent<TMP_InputField>().text = 当前符文树.查找节点(ID).内容;
            B.GetComponent<Image>().color = new Color(195 / 255f, 134 / 255f, 134 / 255f, 255 / 255f);
            游戏物体池.Add(ID, A);
            A.OnStayClick(() => {
                if (正在连线.当前正在连线) {
                    if (正在连线.当前连线的节点 != ID && 正在连线.当前连线的节点 != 当前符文树.查找节点(ID).父节点 && !当前符文树.查找节点(ID).子节点.Contains(正在连线.当前连线的节点)) {
                        DiUpdateDo.Remove("连线跟随鼠标");
                        正在连线.当前正在连线 = false;
                        连线数据 LXS = 正在连线.不定连线;
                        LXS.连线终点 = A.transform.position;
                        连线位置基于坐标调整(LXS.ID);
                        LXS.末节点 = 当前符文树.查找节点(ID);
                        LXS.末节点.父节点 = LXS.初节点.ID;
                        if (正在连线.当前连线的节点的判断条件 == "是") {
                            LXS.初节点.点击是的次数++;
                            LXS.初节点.子节点.Add(LXS.末节点.ID);
                        }
                        if (正在连线.当前连线的节点的判断条件 == "否") {
                            LXS.初节点.点击否的次数++;
                            LXS.初节点.子节点.Add(LXS.末节点.ID);
                        }
                    }
                }
            });
            GameObject C1 = A.创建矩形("0 70% 30% 30%").SetText("是").OnClick(() => {
                if (当前符文树.查找节点(ID).点击是的次数 == 0) {
                        if (!正在连线.当前正在连线) {
                            存档.符文树连线ID++;
                            连线数据 LXS = new 连线数据() { ID = "连线" + 存档.符文树连线ID, 初节点 = 当前符文树.查找节点(ID), 末节点 = 当前符文树.查找节点(ID), 连线起点 = A.transform.position, 连线终点 = A.transform.position, 所属符文树 = 存档.当前符文树 };
                            当前符文树.符文树连线.Add(LXS);
                            连线 LX = new 连线("连线" + 存档.符文树连线ID);
                            正在连线.当前正在连线 = true;
                            正在连线.当前连线的节点 = ID;
                            正在连线.不定连线 = LXS;
                            正在连线.当前连线的节点的判断条件 = "是";
                            DiUpdateDo["连线跟随鼠标"] = () => {
                                Vector3 Ve = Input.mousePosition;
                                Ve.z = 0;
                                LXS.连线终点 = Ve;
                                连线位置基于坐标调整(LXS.ID);
                            };
                        }
                    
                }
            });
            GameObject C2 = A.创建矩形("40% 70% 30% 30%").SetText("否").OnClick(() => {
                if (当前符文树.查找节点(ID).点击否的次数 == 0) {
                        if (!正在连线.当前正在连线) {
                            存档.符文树连线ID++;
                            连线数据 LXS = new 连线数据() { ID = "连线" + 存档.符文树连线ID, 初节点 = 当前符文树.查找节点(ID), 末节点 = 当前符文树.查找节点(ID), 连线起点 = A.transform.position, 连线终点 = A.transform.position, 所属符文树 = 存档.当前符文树 };
                            当前符文树.符文树连线.Add(LXS);
                            连线 LX = new 连线("连线" + 存档.符文树连线ID);
                            正在连线.当前正在连线 = true;
                            正在连线.当前连线的节点 = ID;
                            正在连线.不定连线 = LXS;
                            正在连线.当前连线的节点的判断条件 = "否";
                            DiUpdateDo["连线跟随鼠标"] = () => {
                                Vector3 Ve = Input.mousePosition;
                                Ve.z = 0;
                                LXS.连线终点 = Ve;
                                连线位置基于坐标调整(LXS.ID);
                            };
                        }
                    }
                
            });
            当前符文树.查找节点(ID).节点坐标 = A.transform.position;
            Vector3 offset = Vector3.zero;
            A.OnBeginDrag(() => {
                offset = new Vector3(A.transform.position.x, A.transform.position.y) - Input.mousePosition;
            });
            A.OnDrag(() => {
                A.transform.position = Input.mousePosition + offset;
                当前符文树.查找节点(ID).节点坐标 = A.transform.position;
                //调整关于此节点的线条位置
                foreach (var ins in 存档.已创建的符文树[存档.当前符文树].符文树连线) {
                    连线位置基于节点调整(ins.ID);
                }
            });
        }
    }
    public class 连线 {
        public string ID;
        private 符文树数据 当前符文树;
        public 连线(string Id) {
            ID = Id;
            当前符文树 = 存档.已创建的符文树[存档.当前符文树];
            GameObject A = 两点间连线(当前符文树.查找线条(ID).连线起点, 当前符文树.查找线条(ID).连线起点);
            游戏物体池.Add(ID, A);
            连线位置基于节点调整(ID);
        }
    }
    public class 笔记本 {
        public string ID;
        public 笔记本(string id) {
            ID = id;
            GameObject A = MainPanel.创建矩形("40% 10% 500 1000");
            A.AddComponent<Image>().color = new Color(195 / 255f, 134 / 255f, 134 / 255f, 255 / 255f);
            GameObject B = A.CreateInput("0 0 100% 100%");
            游戏物体池.Add(ID, A);
        }
    }
}
//控制台
namespace 符文树3 {
    public static partial class LocalStorage {
        public static 控制台类 控制台;
        public static void 初始化控制台() {
            控制台 = MainPanel.CreateConsole("80% 0 20% 100%");
            初始化控制台指令();
        }
        public static void 初始化控制台指令() {
            控制台["创建符文树节点"] = t => {
                if (存档.当前符文树 != null) {
                    存档.符文树节点ID++;
                    存档.已创建的符文树[存档.当前符文树].符文树节点.Add(new 节点数据{ ID = "符文树节点" + 存档.符文树节点ID });
                    节点 JD = new 节点("符文树节点" + 存档.符文树节点ID);
                } else {
                    Debug.Log("创建节点前请先选择需要编辑的符文树");
                }
            };
            控制台["创建符文树条目"] = t => {
                存档.符文树条目ID++;
                存档.已创建的符文树.Add("符文树" + 存档.符文树条目ID, new 符文树数据 {数据类别 = "符文树", ID = "符文树" + 存档.符文树条目ID });
                符文树编辑器.刷新();
            };
            控制台["创建笔记本"] = t => {
                存档.笔记本ID++;

                存档.已创建的符文树.Add("笔记本" + 存档.笔记本ID, new 符文树数据 { 数据类别 = "笔记本", ID = "笔记本" + 存档.笔记本ID});
                存档.已创建的符文树["笔记本" + 存档.笔记本ID].符文树节点.Add(new 笔记本数据 {ID= "笔记本数据" + 存档.笔记本ID });
                符文树编辑器.刷新();
            };
            控制台["存档"] = t => {
                执行存档();
            };  
        }
    }
}
//符文树正文
namespace 符文树3 {
    public static partial class LocalStorage {
        public static void Main() {
            读档();
            符文树编辑器 = new 符文树目录();
            初始化控制台();
            UpdateDo = () => {
                if (正在连线.当前正在连线) {
                    foreach (var ins in 存档.已创建的符文树[存档.当前符文树].符文树节点) {
                        if (游戏物体池.ContainsKey(ins.ID)) {
                            if (判断鼠标是否在某UI上(游戏物体池[ins.ID].GetComponent<RectTransform>())) {
                                if (正在连线.当前连线的节点 != ins.ID && 正在连线.当前连线的节点 != 存档.已创建的符文树[存档.当前符文树].查找节点(ins.ID).父节点 && !存档.已创建的符文树[存档.当前符文树].查找节点(ins.ID).子节点.Contains(正在连线.当前连线的节点)) {
                               
                                }
                            }

                        }
                    }
                }
            };
        }
    }
}
