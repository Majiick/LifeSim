using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LSystem : MonoBehaviour {
    struct PosAndRot {
        public Quaternion rot;
        public Vector3 pos;

        public PosAndRot(Vector3 pos, Quaternion rot) {
            this.pos = pos;
            this.rot = rot;
        }
    }

    public LineRenderer lr;
	// Use this for initialization
	void Start () {
	    
	}

    string GetRandomString(int length, string chars) {
        string ret = "";
        Stack<char> bracketStack = new Stack<char>();
        for (int i = 0; i < length; i++) {
            char c = chars[Random.Range(0, chars.Length)];
            if (c == ']') {
                if (bracketStack.Count == 0 || bracketStack.Peek() != '[') {
                    do {
                        c = chars[Random.Range(0, chars.Length)];
                    } while (c == ']' || c == '[');
                }
                else {
                    bracketStack.Push(']');
                }
            } else if (c == '[') {
                if (bracketStack.Count != 0) {
                    if (bracketStack.Peek() == '[') {
                        do {
                            c = chars[Random.Range(0, chars.Length)];
                        } while (c == ']' || c == '[');
                    }
                    else {
                        bracketStack.Push('[');
                    }
                }
                else {
                    bracketStack.Push('[');
                }
            } else if (c == '%') {
                ret += "%" + Random.Range(1, 60).ToString();
            }

            if (c != '%') {
                ret += c;
            }
        }

        if (bracketStack.Count > 0 && bracketStack.Peek() == '[') {
            ret += ']';
        }

        return ret;
    }

    int getNumberAt(string str, int i) {
        string num = "";
        while (i < str.Length && "0123456789".Contains(str[i].ToString())) {
            num += str[i];
            i++;
        }
//        Debug.Log(num);
        return int.Parse(num);
    }
	
	// Update is called once per frame
	void Update () {

	    if (Input.anyKeyDown) {
	        transform.position = Vector3.zero;
//            Debug.Log(GetRandomString(6));
	        float rotation = Random.Range(1f, 360f);
	        // Fern
	        string lSystem = GetRandomString(Random.Range(1, 4), "XFZ");
	        Debug.Log(lSystem);
            Dictionary<string, string> rules = new Dictionary<string, string>() {
	            {"F", GetRandomString(Random.Range(0, 50), "[]FX+-ZQWER")},
	            {"X", GetRandomString(Random.Range(0, 50), "[]FX+-ZQWER")}
//	            {"Z", GetRandomString(Random.Range(0, 20), "[]FX+-ZQWER")},
//                {"Q", GetRandomString(Random.Range(0, 20), "[]FX+-ZQWER")},
//                {"W", GetRandomString(Random.Range(0, 20), "[]FX+-ZQWER")},
//                {"E", GetRandomString(Random.Range(0, 20), "[]FX+-ZQWER")},
//                {"R", GetRandomString(Random.Range(0, 20), "[]FX+-ZQWER")}
            };
	        foreach (var rule in rules) {
	            Debug.Log(rule.Key + ": " + rule.Value);
	        }
            Debug.Log("\n");

            for (int i = 0; i < 6; i++) {
                StringBuilder next = new StringBuilder();
                foreach (char c in lSystem) {
	                if (rules.ContainsKey(c.ToString())) {
	                    next.Append(rules[c.ToString()]);
	                } else {
	                    next.Append(c);
	                }
	            }

	            lSystem = next.ToString();
	        }

	        List<Vector3> positions = new List<Vector3>() { transform.position };
	        Stack<PosAndRot> stack = new Stack<PosAndRot>();
	        for (int i = 0; i < lSystem.Length; i++) {
	            switch (lSystem[i]) {
	                case 'F':
	                    transform.Translate(Vector3.forward, Space.Self);
	                    break;
	                case '-':
	                    transform.Rotate(Vector3.up, -rotation);
	                    break;
	                case '+':
	                    transform.Rotate(Vector3.up, +rotation);
	                    break;
	                case '[':
	                    stack.Push(new PosAndRot(transform.position, transform.rotation));
	                    break;
	                case ']':
	                    var posAndRot = stack.Pop();
	                    transform.position = posAndRot.pos;
	                    transform.rotation = posAndRot.rot;
	                    break;
                    case '%':
                        rotation += getNumberAt(lSystem, i + 1);
                        break;
	            }

	            positions.Add(transform.position);
	        }

	        lr.positionCount = lSystem.Length;
	        lr.SetPositions(positions.ToArray());
        }
	}
}
