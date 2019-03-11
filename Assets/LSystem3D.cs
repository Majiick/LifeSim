using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class StochasticLSystem {
    private Dictionary<string, List<string>> rules = new Dictionary<string, List<string>>();
    private Dictionary<string, List<double>> productionProbabilities = new Dictionary<string, List<double>>();

    public void InsertRule(string production, string rule, double probability) {
        if (!rules.ContainsKey(production)) {
            rules[production] = new List<string>();
            productionProbabilities[production] = new List<double>();
        }

        rules[production].Add(rule);
        productionProbabilities[production].Add(probability);
    }

    private StochasticLSystem() {
        
    }

    public string GetRule(string production) {
        List<string> prodRules = rules[production];
        List<double> probabilities = productionProbabilities[production];
        Debug.Assert(prodRules.Count == probabilities.Count);

        List<double> ranges = new List<double>();  // If 3 rules with 0.25, 0.5, and 0.25 probability, then ranges will be 0.25, 0.75, 1.0 
                                                   // Then take the random number and pick the prodRule index that is below the ranges[index] when iterating.
        double maxRandomNum = 0;
        foreach (double probability in probabilities) {
            maxRandomNum += probability;
            ranges.Add(maxRandomNum);
        }
        Debug.Assert(maxRandomNum <= 1.0f);

        double randomNum = Random.Range(0, (float)maxRandomNum);
        for (int i = 0; i < prodRules.Count; i++) {
            if (randomNum <= ranges[i]) {
                return prodRules[i];
            }
        }

        Debug.Assert(false); // Should never happen
        return null;
    }
}

public class LSystem3D : MonoBehaviour {
    public GameObject rotateAround;
    private List<Vector3> savedPositions;
    private int segmentIterator = 0;

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
    void Start() {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.Rotate(new Vector3(-90, 0, 0));
        float rotation = 22.5f;

        string lSystem = "A";
        Dictionary<string, string> rules = new Dictionary<string, string>() {
                {"A", "[&FL!A]/////’[&FL!A]///////’[&FL!A]"},
                {"F",  "S/////F"},
                {"S", "FL"},
                {"L", "[’’’∧∧{-f+f+f-|-f+f+f}]"}
            };
        foreach (var rule in rules) {
            Debug.Log(rule.Key + ": " + rule.Value);
        }
        Debug.Log("\n");

        for (int i = 0; i < 8; i++) {
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
                case 'f':
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
                case '/':
                    transform.Rotate(Vector3.forward, rotation);
                    break;
                case ')':
                    transform.Rotate(Vector3.forward, -rotation);
                    break;
                case '&':
                    transform.Rotate(Vector3.right, rotation);
                    break;
                case '^':
                    transform.Rotate(Vector3.right, -rotation);
                    break;
                case '|':
                    transform.Rotate(Vector3.up, 180f);
                    break;
            }

            positions.Add(transform.position);
        }

        savedPositions = positions;
    }

    // Update is called once per frame
    void Update() {
        if (Input.anyKeyDown) {
            
        }

        Camera.main.transform.RotateAround(rotateAround.transform.position, Vector3.up, 20 * Time.deltaTime);

        lr.SetPositions(savedPositions.ToArray());
        lr.positionCount = segmentIterator;
        segmentIterator += 200;
    }
}
